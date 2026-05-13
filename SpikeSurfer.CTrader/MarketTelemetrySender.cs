using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeSurfer.CTrader;

public class MarketTelemetrySender
{
    private readonly HttpClient _http = new();

    public async Task SendAsync(string url, MarketTelemetrySnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(snapshot);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _http.PostAsync(url, content);
    }
}

public class MarketTelemetrySnapshot
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
}