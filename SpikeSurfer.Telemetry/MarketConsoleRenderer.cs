using SpikeSurfer.Models;

namespace SpikeSurfer.Telemetry;

public sealed class MarketConsoleRenderer
{
    private string? _lastState;

    public void Render(ReplaySnapshot snapshot)
    {
        Console.Clear();

        var stateChanged = _lastState is not null && _lastState != snapshot.MarketState;

        if (stateChanged)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"STATE CHANGED: {_lastState} -> {snapshot.MarketState}");
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine($"[{snapshot.TimestampUtc:HH:mm:ss}] {snapshot.Symbol}");
        Console.WriteLine();

        WriteLine("STATE", snapshot.MarketState);
        WriteLine("DECISION", snapshot.Decision);
        WriteLine("CONFIDENCE", snapshot.Confidence.ToString());

        Console.WriteLine();
        Console.WriteLine("ENGINE SCORES");
        Console.WriteLine($"ExpansionStrength     : {snapshot.EngineScores.ExpansionStrength:0.00}");
        Console.WriteLine($"ExpansionPersistence  : {snapshot.EngineScores.ExpansionPersistence:0.00}");
        Console.WriteLine($"ExhaustionScore       : {snapshot.EngineScores.ExhaustionScore:0.00}");
        Console.WriteLine($"FailureDetected       : {snapshot.EngineScores.FailureDetected}");
        Console.WriteLine($"ContextScore          : {snapshot.EngineScores.ContextScore:0.00}");
        Console.WriteLine($"H1TrendConsistency    : {snapshot.EngineScores.H1TrendConsistency}");
        Console.WriteLine($"H4TrendConsistency    : {snapshot.EngineScores.H4TrendConsistency}");

        Console.WriteLine();
        Console.WriteLine("REASON");

        foreach (var reason in snapshot.Reason)
            Console.WriteLine($"- {reason}");

        _lastState = snapshot.MarketState;
    }

    private static void WriteLine(string label, string value)
    {
        Console.Write(label.PadRight(14));
        Console.Write(": ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(value);
        Console.ResetColor();
    }
}