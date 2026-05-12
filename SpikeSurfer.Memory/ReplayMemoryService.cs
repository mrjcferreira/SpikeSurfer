using System.Text.Json;
using SpikeSurfer.Models;

namespace SpikeSurfer.Memory;

public sealed class ReplayMemoryService
{
    private readonly string _basePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public ReplayMemoryService(string basePath = "data/replays")
    {
        _basePath = basePath;

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        Directory.CreateDirectory(_basePath);
    }

    public async Task SaveAsync(ReplaySnapshot snapshot, CancellationToken cancellationToken = default)
    {
        if (snapshot is null)
            throw new ArgumentNullException(nameof(snapshot));

        var fileName = $"{Sanitize(snapshot.Id)}.json";
        var path = Path.Combine(_basePath, fileName);

        var json = JsonSerializer.Serialize(snapshot, _jsonOptions);

        await File.WriteAllTextAsync(path, json, cancellationToken);
    }

    public async Task<ReplaySnapshot?> LoadAsync(string id, CancellationToken cancellationToken = default)
    {
        var fileName = $"{Sanitize(id)}.json";
        var path = Path.Combine(_basePath, fileName);

        if (!File.Exists(path))
            return null;

        var json = await File.ReadAllTextAsync(path, cancellationToken);

        return JsonSerializer.Deserialize<ReplaySnapshot>(json, _jsonOptions);
    }

    public async Task<IReadOnlyList<ReplaySnapshot>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(_basePath, "*.json");

        var results = new List<ReplaySnapshot>();

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file, cancellationToken);
            var snapshot = JsonSerializer.Deserialize<ReplaySnapshot>(json, _jsonOptions);

            if (snapshot is not null)
                results.Add(snapshot);
        }

        return results
            .OrderByDescending(x => x.TimestampUtc)
            .ToList();
    }

    private static string Sanitize(string value)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars())
            value = value.Replace(invalid, '_');

        return value;
    }
}