using System.Text;
using System.Text.Json;

namespace SpikeSurfer.CTrader;

public sealed class MarketTelemetrySender
{
    private readonly HttpClient _http = new();

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task SendAsync(string url, MarketTelemetryPayload payload)
    {
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            // Dashboard unreachable — swallow silently so cBot doesn't crash.
            // TODO: Add logging / retry counter.
        }
    }
}

public class MarketTelemetryPayload
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
