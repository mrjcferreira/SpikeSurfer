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

    private readonly PushcutNotifier _pushcut;

    public MarketController(PushcutNotifier pushcut)
    {
        _pushcut = pushcut;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] MarketTelemetryData snapshot)
    {
        lock (SyncLock)
        {
            snapshot.ReceivedAtUtc = DateTime.UtcNow;
            LastSnapshot = snapshot;

            if (snapshot.Mid > SessionHigh) SessionHigh = snapshot.Mid;
            if (snapshot.Mid < SessionLow) SessionLow = snapshot.Mid;
        }

        // Evaluate Pushcut alert (async, outside lock)
        await _pushcut.EvaluateAndNotifyAsync(new MarketAlertData
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
        });

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

    [HttpGet("health")]
    public IActionResult Health()
    {
        lock (SyncLock)
        {
            var hasData = LastSnapshot is not null;
            var age = hasData
                ? (DateTime.UtcNow - LastSnapshot!.ReceivedAtUtc).TotalSeconds
                : -1;

            return Ok(new
            {
                status = hasData && age < 10 ? "healthy" : hasData ? "stale" : "waiting",
                lastUpdateSecondsAgo = Math.Round(age, 1),
                symbol = LastSnapshot?.Symbol ?? "N/A"
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
        }

        return Ok(new { status = "Session stats reset" });
    }
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