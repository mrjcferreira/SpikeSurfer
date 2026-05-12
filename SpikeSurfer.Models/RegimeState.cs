namespace SpikeSurfer.Models;

public sealed record RegimeState(
    MarketRegime Regime,
    double Confidence,
    string Reason
);