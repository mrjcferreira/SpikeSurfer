using SpikeSurfer.Models;

namespace SpikeSurfer.Engines;

public sealed class SecondWaveReaperEngine
{
    public StrategyState Analyze(MarketSnapshot snapshot, OpenRangeState openRange)
    {
        var candles = snapshot.M1;

        if (candles.Count < 25)
            return new StrategyState("SECOND_WAVE_REAPER", 0, "Not enough candles", ExecutionIntent.Watch);

        var last = candles[^1];
        var previous = candles[^2];

        var rejectionShort =
            openRange.BrokeHigh &&
            last.High > openRange.High &&
            last.Close < last.Open &&
            last.Close < openRange.High;

        var failedContinuation =
            previous.High > openRange.High &&
            last.Close < previous.Close;

        var secondWave =
            candles.Count(c => c.High > openRange.High) >= 2;

        if (openRange.FalseBreakHigh && rejectionShort && failedContinuation && secondWave)
        {
            return new StrategyState(
                "SECOND_WAVE_REAPER",
                88,
                "Second wave failed above open range: short trap detected",
                ExecutionIntent.ExecuteShort
            );
        }

        if (openRange.BrokeHigh)
        {
            return new StrategyState(
                "SECOND_WAVE_REAPER",
                55,
                "First/uncertain wave above open range: wait",
                ExecutionIntent.Watch
            );
        }

        return new StrategyState(
            "SECOND_WAVE_REAPER",
            20,
            "No reaper setup",
            ExecutionIntent.Watch
        );
    }
}