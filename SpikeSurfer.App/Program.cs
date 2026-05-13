using SpikeSurfer.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Pushcut notifications
var pushcutUrl = builder.Configuration["Pushcut:WebhookUrl"]
    ?? throw new InvalidOperationException("Pushcut:WebhookUrl not configured in appsettings.json");

builder.Services.AddSingleton(new PushcutNotifier(pushcutUrl, new PushcutThresholds
{
    InfoPercent = 0.1,
    WarningPercent = 0.3,
    CriticalPercent = 0.5,
    CooldownSeconds = 120,
    MinPriceMoveBetweenAlerts = 1.0
}));

// Gemini sentiment analysis
var geminiKey = builder.Configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException("Gemini:ApiKey not configured in appsettings.json");

var geminiModel = builder.Configuration["Gemini:Model"] ?? "gemini-2.5-flash";
var geminiInterval = int.TryParse(builder.Configuration["Gemini:IntervalSeconds"], out var gi) ? gi : 300;

builder.Services.AddSingleton(new GeminiSentimentService(geminiKey, geminiModel, geminiInterval));

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => new
{
    status = "SpikeSurfer API running",
    timeUtc = DateTime.UtcNow
});

app.Run();