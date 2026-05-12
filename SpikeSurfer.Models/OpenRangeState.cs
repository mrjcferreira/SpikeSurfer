namespace SpikeSurfer.Models;

public sealed record OpenRangeState(
    bool IsActive,
    decimal High,
    decimal Low,
    bool BrokeHigh,
    bool BrokeLow,
    bool FalseBreakHigh,
    bool FalseBreakLow,
    double Confidence,
    string Reason
);