var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => new
{
    status = "SpikeSurfer API running",
    timeUtc = DateTime.UtcNow
});

app.Run();