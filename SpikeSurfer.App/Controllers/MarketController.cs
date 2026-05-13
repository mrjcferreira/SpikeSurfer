using Microsoft.AspNetCore.Mvc;
using SpikeSurfer.App.Services;

namespace SpikeSurfer.App.Controllers;

[ApiController]
[Route("api/market")]
public class MarketController : ControllerBase
{
    private static readonly object SyncLock = new();
    private static MarketTelemetryData? LastSnapshot;
    private static double SessionHigh = double.MinValue;
    private static double SessionLow = double.MaxValue;

    private static readonly CandleAccumulator M1 = new(1, 120);
    private static readonly CandleAccumulator M5 = new(5, 60);
    private static readonly CandleAccumulator M15 = new(15, 40);

    private readonly PushcutNotifier _pushcut;
    private readonly GeminiSentimentService _gemini;

    public MarketController(PushcutNotifier pushcut, GeminiSentimentService gemini)
    {
        _pushcut = pushcut;
        _gemini = gemini;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] MarketTelemetryData snapshot)
    {
        MarketAlertData alertData;

        lock (SyncLock)
        {
            snapshot.ReceivedAtUtc = DateTime.UtcNow;
            LastSnapshot = snapshot;

            if (snapshot.Mid > SessionHigh) SessionHigh = snapshot.Mid;
            if (snapshot.Mid < SessionLow) SessionLow = snapshot.Mid;

            M1.AddTick(snapshot.Mid, snapshot.ServerTimeUtc);
            M5.AddTick(snapshot.Mid, snapshot.ServerTimeUtc);
            M15.AddTick(snapshot.Mid, snapshot.ServerTimeUtc);

            alertData = new MarketAlertData
            {
                Bid = snapshot.Bid,
                Ask = snapshot.Ask,
                Mid = snapshot.Mid,
                Price1hAgo = snapshot.Price1hAgo,
                Change1h = snapshot.Change1h,
                Change1hPercent = snapshot.Change1hPercent,
                Spread = snapshot.Spread,
                Session = snapshot.Session,
                ServerTimeUtc = snapshot.ServerTimeUtc,
                SessionHigh = SessionHigh,
                SessionLow = SessionLow,
                DashboardUrl = $"{Request.Scheme}://{Request.Host}/dashboard"
            };
        }

        // Fire and forget — don't block tick updates
        _ = _pushcut.EvaluateAndNotifyAsync(alertData);
        _ = _gemini.GetSentimentAsync(alertData);

        return Ok(new { status = "OK" });
    }

    [HttpGet("xauusd")]
    public IActionResult GetGold()
    {
        lock (SyncLock)
        {
            if (LastSnapshot is null)
                return NotFound(new { error = "No market data received yet" });

            return Ok(LastSnapshot);
        }
    }

    [HttpGet("candles/{timeframe}")]
    public IActionResult GetCandles(string timeframe)
    {
        lock (SyncLock)
        {
            var acc = timeframe.ToLower() switch
            {
                "m1" => M1, "m5" => M5, "m15" => M15, _ => null
            };
            if (acc is null)
                return BadRequest(new { error = "Use m1, m5, or m15" });

            return Ok(acc.GetCandles());
        }
    }

    [HttpGet("sentiment")]
    public IActionResult GetSentiment()
    {
        var result = _gemini.GetCachedResult();
        if (result is null)
            return NotFound(new { error = "No sentiment data yet. Waiting for first analysis..." });

        return Ok(result);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        lock (SyncLock)
        {
            var hasData = LastSnapshot is not null;
            var age = hasData ? (DateTime.UtcNow - LastSnapshot!.ReceivedAtUtc).TotalSeconds : -1;
            var sentiment = _gemini.GetCachedResult();

            return Ok(new
            {
                status = hasData && age < 10 ? "healthy" : hasData ? "stale" : "waiting",
                lastUpdateSecondsAgo = Math.Round(age, 1),
                symbol = LastSnapshot?.Symbol ?? "N/A",
                candles = new { m1 = M1.Count, m5 = M5.Count, m15 = M15.Count },
                sentiment = sentiment?.Sentiment ?? "N/A",
                sentimentAge = sentiment is not null ? Math.Round((DateTime.UtcNow - sentiment.Timestamp).TotalSeconds, 0) + "s" : "N/A"
            });
        }
    }

    [HttpPost("reset-session")]
    public IActionResult ResetSession()
    {
        lock (SyncLock)
        {
            SessionHigh = double.MinValue;
            SessionLow = double.MaxValue;
            M1.Clear(); M5.Clear(); M15.Clear();
        }
        return Ok(new { status = "Session stats reset" });
    }
}

// === Models ===

public class CandleAccumulator
{
    private readonly int _intervalMinutes;
    private readonly int _maxCandles;
    private readonly List<CandleData> _candles = new();
    private CandleData? _current;
    private DateTime _currentClose = DateTime.MinValue;
    public int Count => _candles.Count + (_current is not null ? 1 : 0);

    public CandleAccumulator(int intervalMinutes, int maxCandles)
    {
        _intervalMinutes = intervalMinutes;
        _maxCandles = maxCandles;
    }

    public void AddTick(double price, DateTime serverTime)
    {
        var minute = serverTime.Minute - (serverTime.Minute % _intervalMinutes);
        var candleOpen = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day,
            serverTime.Hour, minute, 0, DateTimeKind.Utc);
        var candleClose = candleOpen.AddMinutes(_intervalMinutes);

        if (_current is null || serverTime >= _currentClose)
        {
            if (_current is not null)
            {
                _candles.Add(_current);
                while (_candles.Count > _maxCandles) _candles.RemoveAt(0);
            }
            _current = new CandleData
            {
                Time = candleOpen.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Open = price, High = price, Low = price, Close = price
            };
            _currentClose = candleClose;
        }
        else
        {
            if (price > _current.High) _current.High = price;
            if (price < _current.Low) _current.Low = price;
            _current.Close = price;
        }
    }

    public List<CandleData> GetCandles()
    {
        var r = new List<CandleData>(_candles);
        if (_current is not null) r.Add(_current);
        return r;
    }

    public void Clear() { _candles.Clear(); _current = null; _currentClose = DateTime.MinValue; }
}

public class MarketTelemetryData
{
    public string Symbol { get; set; } = "XAUUSD";
    public double Bid { get; set; }
    public double Ask { get; set; }
    public double Mid { get; set; }
    public double Price1hAgo { get; set; }
    public double Change1h { get; set; }
    public double Change1hPercent { get; set; }
    public double Spread { get; set; }
    public string Session { get; set; } = "";
    public DateTime ServerTimeUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
}

public class CandleData
{
    public string Time { get; set; } = "";
    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
}