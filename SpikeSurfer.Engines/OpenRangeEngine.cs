using SpikeSurfer.Models;

namespace SpikeSurfer.Engines;

public sealed class OpenRangeEngine
{
    public OpenRangeState Analyze(MarketSnapshot snapshot)
    {
        var candles = snapshot.M1;

        if (candles.Count < 20)
        {
            return new OpenRangeState(false, 0, 0, false, false, false, false, 0, "Not enough candles for open range");
        }

        var rangeCandles = candles.Take(15).ToList();
        var recent = candles.Last();

        var high = rangeCandles.Max(c => c.High);
        var low = rangeCandles.Min(c => c.Low);

        var brokeHigh = recent.High > high;
        var brokeLow = recent.Low < low;

        var falseBreakHigh = brokeHigh && recent.Close < high;
        var falseBreakLow = brokeLow && recent.Close > low;

        var confidence =
            falseBreakHigh || falseBreakLow ? 80 :
            brokeHigh || brokeLow ? 55 :
            30;

        var reason =
            falseBreakHigh ? "False break above open range high" :
            falseBreakLow ? "False break below open range low" :
            brokeHigh ? "Break above open range high" :
            brokeLow ? "Break below open range low" :
            "Price inside open range";

        return new OpenRangeState(
            true,
            high,
            low,
            brokeHigh,
            brokeLow,
            falseBreakHigh,
            falseBreakLow,
            confidence,
            reason
        );
    }
}