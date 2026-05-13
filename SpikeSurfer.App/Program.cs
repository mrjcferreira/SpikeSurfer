using SpikeSurfer.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Pushcut notifications — URL from appsettings.json
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

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => new
{
    status = "SpikeSurfer API running",
    timeUtc = DateTime.UtcNow
});

app.Run();