using System.Text;
using System.Text.Json;

namespace SpikeSurfer.App.Services;

public sealed class PushcutNotifier
{
    private readonly HttpClient _http = new();
    private readonly string _webhookUrl;
    private readonly PushcutThresholds _thresholds;

    private DateTime _lastNotification = DateTime.MinValue;
    private double _lastNotifiedMid;

    public PushcutNotifier(string webhookUrl, PushcutThresholds? thresholds = null)
    {
        _webhookUrl = webhookUrl;
        _thresholds = thresholds ?? new PushcutThresholds();
    }

    /// <summary>
    /// Evaluates the current market data and sends a Pushcut notification
    /// if thresholds are breached and cooldown has elapsed.
    /// </summary>
    public async Task EvaluateAndNotifyAsync(MarketAlertData data)
    {
        var dominated = Math.Abs(data.Change1hPercent);
        var sinceLastNotification = DateTime.UtcNow - _lastNotification;

        // Cooldown check — don't spam
        if (sinceLastNotification.TotalSeconds < _thresholds.CooldownSeconds)
            return;

        // Determine alert level
        var alert = ClassifyAlert(data);

        if (alert == AlertLevel.None)
            return;

        // Don't re-notify for the same price zone
        if (Math.Abs(data.Mid - _lastNotifiedMid) < _thresholds.MinPriceMoveBetweenAlerts)
            return;

        var notification = BuildNotification(data, alert);

        try
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(notification, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _http.PostAsync(_webhookUrl, content);

            _lastNotification = DateTime.UtcNow;
            _lastNotifiedMid = data.Mid;
        }
        catch (HttpRequestException)
        {
            // Pushcut unreachable — swallow so engine continues.
        }
    }

    private AlertLevel ClassifyAlert(MarketAlertData data)
    {
        var absChange = Math.Abs(data.Change1hPercent);

        if (absChange >= _thresholds.CriticalPercent)
            return AlertLevel.Critical;

        if (absChange >= _thresholds.WarningPercent)
            return AlertLevel.Warning;

        if (absChange >= _thresholds.InfoPercent)
            return AlertLevel.Info;

        return AlertLevel.None;
    }

    private PushcutPayload BuildNotification(MarketAlertData data, AlertLevel alert)
    {
        var direction = data.Change1hPercent >= 0 ? "▲" : "▼";
        var directionWord = data.Change1hPercent >= 0 ? "UP" : "DOWN";
        var emoji = alert switch
        {
            AlertLevel.Critical => "🚨",
            AlertLevel.Warning => "⚠️",
            _ => "📊"
        };

        var pctFormatted = data.Change1hPercent >= 0
            ? $"+{data.Change1hPercent:F3}%"
            : $"{data.Change1hPercent:F3}%";

        var title = $"{emoji} XAUUSD {direction} {data.Mid:F2}";

        var lines = new List<string>
        {
            $"Price: {data.Mid:F2} ({pctFormatted})",
            $"Move: {directionWord} {Math.Abs(data.Change1h):F2} pts in 1h",
            $"Bid: {data.Bid:F2}  Ask: {data.Ask:F2}  Spread: {data.Spread:F1}",
            $"Session: {data.Session}",
            $"Alert: {alert}",
            $"Server: {data.ServerTimeUtc:HH:mm:ss} UTC"
        };

        if (data.SessionHigh > 0)
            lines.Add($"Range: {data.SessionLow:F2} — {data.SessionHigh:F2}");

        if (!string.IsNullOrEmpty(data.Regime))
            lines.Add($"Regime: {data.Regime}");

        if (!string.IsNullOrEmpty(data.Signal))
            lines.Add($"Signal: {data.Signal}");

        var text = string.Join("\n", lines);

        var payload = new PushcutPayload
        {
            Title = title,
            Text = text,
            Sound = alert == AlertLevel.Critical ? "horn" : "default",
            IsTimeSensitive = alert >= AlertLevel.Warning
        };

        // Attach dashboard URL as default action
        if (!string.IsNullOrEmpty(data.DashboardUrl))
        {
            payload.DefaultAction = new PushcutAction
            {
                Name = "Open Dashboard",
                Url = data.DashboardUrl
            };
        }

        // Chart image if available
        if (!string.IsNullOrEmpty(data.ChartImageUrl))
        {
            payload.Image = data.ChartImageUrl;
        }

        return payload;
    }
}

// === Alert classification ===

public enum AlertLevel
{
    None,
    Info,
    Warning,
    Critical
}

// === Configuration ===

public class PushcutThresholds
{
    /// <summary>Minimum % change (1h) to trigger Info alert.</summary>
    public double InfoPercent { get; set; } = 0.1;

    /// <summary>Minimum % change (1h) to trigger Warning alert.</summary>
    public double WarningPercent { get; set; } = 0.3;

    /// <summary>Minimum % change (1h) to trigger Critical alert.</summary>
    public double CriticalPercent { get; set; } = 0.5;

    /// <summary>Minimum seconds between notifications (anti-spam).</summary>
    public int CooldownSeconds { get; set; } = 120;

    /// <summary>Minimum price move between notifications to avoid re-alerting same zone.</summary>
    public double MinPriceMoveBetweenAlerts { get; set; } = 1.0;
}

// === Data models ===

public class MarketAlertData
{
    public double Bid { get; set; }
    public double Ask { get; set; }
    public double Mid { get; set; }
    public double Price1hAgo { get; set; }
    public double Change1h { get; set; }
    public double Change1hPercent { get; set; }
    public double Spread { get; set; }
    public string Session { get; set; } = "";
    public DateTime ServerTimeUtc { get; set; }

    // Optional enrichment from engines
    public double SessionHigh { get; set; }
    public double SessionLow { get; set; }
    public string? Regime { get; set; }
    public string? Signal { get; set; }

    // Dashboard link for notification action
    public string? DashboardUrl { get; set; }

    // Chart screenshot URL (if you generate one)
    public string? ChartImageUrl { get; set; }
}

// === Pushcut API payload ===

public class PushcutPayload
{
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public string? Sound { get; set; }
    public string? Image { get; set; }
    public bool IsTimeSensitive { get; set; }
    public PushcutAction? DefaultAction { get; set; }
}

public class PushcutAction
{
    public string Name { get; set; } = "";
    public string? Url { get; set; }
}