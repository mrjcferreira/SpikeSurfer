using cAlgo.API;
using SpikeSurfer.Models;

namespace SpikeSurfer.CTrader;

public static class CTraderCandleMapper
{
    public static List<Candle> FromBars(Bars bars, int count = 100)
    {
        var candles = new List<Candle>();

        if (bars is null || bars.Count == 0)
            return candles;

        var take = Math.Min(count, bars.Count);

        for (var i = bars.Count - take; i < bars.Count; i++)
        {
            candles.Add(new Candle(
                Time: bars.OpenTimes[i],
                Open: (decimal)bars.OpenPrices[i],
                High: (decimal)bars.HighPrices[i],
                Low: (decimal)bars.LowPrices[i],
                Close: (decimal)bars.ClosePrices[i],
                Volume: (decimal)bars.TickVolumes[i]
            ));
        }

        return candles;
    }
}