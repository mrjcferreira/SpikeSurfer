using SpikeSurfer.Models;

namespace SpikeSurfer.Engines;

public sealed class StrategyEngine
{
    public StrategyState Select(RegimeState regime)
    {
        return regime.Regime switch
        {
            MarketRegime.OpenRange => new StrategyState("OPEN_RANGE", 70, "Normal market: focus on London/New York open range", ExecutionIntent.Watch),
            MarketRegime.Exhaustion => new StrategyState("EXHAUSTION_WAVE_8000", 85, "Exhaustion market: focus on large reversal wave", ExecutionIntent.ExecuteLong),
            MarketRegime.Spike => new StrategyState("SPIKE_SURF", 75, "Spike market: wait for continuation or failed continuation", ExecutionIntent.Watch),
            MarketRegime.Reversal => new StrategyState("REVERSAL_AFTER_FAILURE", 80, "Reversal market: structure failure after expansion", ExecutionIntent.Watch),
            _ => new StrategyState("NORMAL_MARKET", 50, "Default: observe and wait for structure", ExecutionIntent.Watch)
        };
    }
}