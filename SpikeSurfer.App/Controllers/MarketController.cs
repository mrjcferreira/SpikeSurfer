using Microsoft.AspNetCore.Mvc;

namespace SpikeSurfer.App.Controllers;

[ApiController]
[Route("api/market")]
public class MarketController : ControllerBase
{
    private static readonly object LockObj = new();

    private static MarketSnapshot? LastSnapshot;

    [HttpPost("update")]
    public IActionResult Update([FromBody] MarketSnapshot snapshot)
    {
        lock (LockObj)
        {
            snapshot.ReceivedAtUtc = DateTime.UtcNow;
            LastSnapshot = snapshot;
        }

        return Ok(new
        {
            status = "OK"
        });
    }

    [HttpGet("xauusd")]
    public IActionResult GetGold()
    {
        lock (LockObj)
        {
            if (LastSnapshot == null)
            {
                return NotFound(new
                {
                    error = "No market data received yet"
                });
            }

            return Ok(LastSnapshot);
        }
    }
}

public class MarketSnapshot
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