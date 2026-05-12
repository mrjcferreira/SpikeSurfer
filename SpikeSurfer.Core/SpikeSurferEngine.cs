namespace SpikeSurfer.Core;

using SpikeSurfer.Models;
using SpikeSurfer.Engines;

public sealed class SpikeSurferEngine
{
    public MarketReadResult Read(MarketSnapshot snapshot)
{
    var regimeEngine = new RegimeEngine();
    var regime = regimeEngine.Analyze(snapshot);

    var strategyEngine = new StrategyEngine();
    var strategy = strategyEngine.Select(regime);

        return new MarketReadResult(
            new MarketContext("UNKNOWN", regime.Reason, false, false, false, false),
            new WaveState(0, 0, 0, false, false, false, "BOOT"),
            new StructureState(0, 0, "NONE", "NONE", false, false, false, false, false, false, 0),
            new ExhaustionState(0, 0, false, false, false, false, false, false, "NONE"),
            new RiskState(false, 0, 0m, 0m),
            new AiValidation(false, strategy.Confidence, strategy.Reason),
            strategy.Intent
            );
}
}