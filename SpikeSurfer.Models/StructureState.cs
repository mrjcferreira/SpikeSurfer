namespace SpikeSurfer.Models;

public sealed record StructureState(
    int H1TrendConsistency,
    int H4TrendConsistency,
    string H1Structure,
    string H4Structure,
    bool H1Bullish,
    bool H1Bearish,
    bool H4Bullish,
    bool H4Bearish,
    bool AllowsShort,
    bool AllowsLong,
    double ContextScore
);