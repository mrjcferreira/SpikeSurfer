namespace SpikeSurfer.Models;

public sealed record RiskState(
    bool RiskApproved,
    double RiskScore,
    decimal SuggestedStopLoss,
    decimal SuggestedTakeProfit
);