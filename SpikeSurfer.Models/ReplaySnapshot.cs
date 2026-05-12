namespace SpikeSurfer.Models;

public sealed record ReplaySnapshot(
    string Id,
    string Symbol,
    DateTime TimestampUtc,
    string MarketState,
    string Decision,
    int Confidence,
    EngineReplayScores EngineScores,
    IReadOnlyList<string> Reason,
    ReplayOutcome? Outcome
);

public sealed record EngineReplayScores(
    double ExpansionStrength,
    double ExpansionPersistence,
    double ExhaustionScore,
    bool FailureDetected,
    double ContextScore,
    int H1TrendConsistency,
    int H4TrendConsistency
);