namespace SpikeSurfer.Models;

public sealed record StrategyState(
    string Name,
    double Confidence,
    string Reason,
    ExecutionIntent Intent
);