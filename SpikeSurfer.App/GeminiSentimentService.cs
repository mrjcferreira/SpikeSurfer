using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpikeSurfer.App.Services;

public sealed class GeminiSentimentService
{
    private readonly HttpClient _http = new();
    private readonly string _apiKey;
    private readonly string _model;
    private readonly int _intervalSeconds;

    private DateTime _lastCall = DateTime.MinValue;
    private GeminiSentimentResult? _lastResult;
    private readonly object _lock = new();

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public GeminiSentimentService(string apiKey, string model = "gemini-2.5-flash", int intervalSeconds = 300)
    {
        _apiKey = apiKey;
        _model = model;
        _intervalSeconds = intervalSeconds;
    }

    /// <summary>
    /// Returns cached result or calls Gemini if enough time has elapsed.
    /// </summary>
    public async Task<GeminiSentimentResult?> GetSentimentAsync(MarketAlertData market)
    {
        var elapsed = (DateTime.UtcNow - _lastCall).TotalSeconds;

        if (elapsed < _intervalSeconds && _lastResult is not null)
            return _lastResult;

        try
        {
            var result = await CallGeminiAsync(market);
            lock (_lock)
            {
                _lastResult = result;
                _lastCall = DateTime.UtcNow;
            }
            return result;
        }
        catch
        {
            return _lastResult; // Return stale data on failure
        }
    }

    public GeminiSentimentResult? GetCachedResult()
    {
        lock (_lock) { return _lastResult; }
    }

    private async Task<GeminiSentimentResult?> CallGeminiAsync(MarketAlertData market)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var prompt = $@"You are a gold (XAUUSD) market analyst. Analyze the current market conditions and provide sentiment analysis.

Current Market Data:
- Price: {market.Mid:F2}
- 1h Change: {market.Change1h:F2} ({market.Change1hPercent:F3}%)
- Bid: {market.Bid:F2} / Ask: {market.Ask:F2}
- Spread: {market.Spread:F1}
- Session: {market.Session}
- Session High: {market.SessionHigh:F2} / Low: {market.SessionLow:F2}
- Server Time: {market.ServerTimeUtc:yyyy-MM-dd HH:mm} UTC

Respond ONLY with a valid JSON object (no markdown, no backticks, no preamble):
{{
  ""sentiment"": ""BULLISH"" or ""BEARISH"" or ""NEUTRAL"",
  ""confidence"": 0-100,
  ""summary"": ""One sentence market summary"",
  ""keyDrivers"": [""driver1"", ""driver2"", ""driver3""],
  ""newsEvents"": [
    {{""title"": ""Event title"", ""impact"": ""HIGH"" or ""MEDIUM"" or ""LOW"", ""bias"": ""BULLISH"" or ""BEARISH"" or ""NEUTRAL""}}
  ],
  ""shortTermOutlook"": ""1-2 sentence outlook for next 1-4 hours"",
  ""riskLevel"": ""LOW"" or ""MODERATE"" or ""HIGH"" or ""EXTREME""
}}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseJson, JsonOpts);

        var text = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        if (string.IsNullOrEmpty(text))
            return null;

        // Clean potential markdown fencing
        text = text.Trim();
        if (text.StartsWith("```")) text = text.Substring(text.IndexOf('\n') + 1);
        if (text.EndsWith("```")) text = text.Substring(0, text.LastIndexOf("```"));
        text = text.Trim();

        var result = JsonSerializer.Deserialize<GeminiSentimentResult>(text, JsonOpts);
        if (result is not null)
            result.Timestamp = DateTime.UtcNow;

        return result;
    }
}

// === Gemini API response models ===

public class GeminiApiResponse
{
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    public GeminiContent? Content { get; set; }
}

public class GeminiContent
{
    public List<GeminiPart>? Parts { get; set; }
}

public class GeminiPart
{
    public string? Text { get; set; }
}

// === Sentiment result model ===

public class GeminiSentimentResult
{
    public string Sentiment { get; set; } = "NEUTRAL";
    public int Confidence { get; set; }
    public string Summary { get; set; } = "";
    public List<string> KeyDrivers { get; set; } = new();
    public List<NewsEvent> NewsEvents { get; set; } = new();
    public string ShortTermOutlook { get; set; } = "";
    public string RiskLevel { get; set; } = "MODERATE";
    public DateTime Timestamp { get; set; }
}

public class NewsEvent
{
    public string Title { get; set; } = "";
    public string Impact { get; set; } = "LOW";
    public string Bias { get; set; } = "NEUTRAL";
}