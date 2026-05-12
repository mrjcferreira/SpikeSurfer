using SpikeSurfer.Models;

namespace SpikeSurfer.Engines;

public sealed class RegimeEngine
{
    public RegimeState Analyze(MarketSnapshot snapshot)
    {
        return new RegimeState(
            MarketRegime.Exhaustion,
            80,
            "TEST: exhaustion regime forced"
        );
    }
}