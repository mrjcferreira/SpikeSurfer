using cAlgo.API;
using SpikeSurfer.Models;

namespace SpikeSurfer.CTrader;

public static class CTraderSnapshotBuilder
{
    public static MarketSnapshot Build(
        Bars m1,
        Bars m5,
        Bars m15,
        Bars h1,
        Bars h4)
    {
        return new MarketSnapshot(
            TimestampUtc: DateTime.UtcNow,

            CurrentPrice: (decimal)m1.ClosePrices.LastValue,

            Spread:
                (decimal)(
                    SymbolProvider
                        .GetSymbol(m1.SymbolName)
                        .Spread
                ),

            Atr:
                CalculateAtr(m1, 14),

            M1Candles:
                CTraderCandleMapper.FromBars(m1, 120),

            M5Candles:
                CTraderCandleMapper.FromBars(m5, 120),

            M15Candles:
                CTraderCandleMapper.FromBars(m15, 120),

            H1Candles:
                CTraderCandleMapper.FromBars(h1, 120),

            H4Candles:
                CTraderCandleMapper.FromBars(h4, 120)
        );
    }

    private static decimal CalculateAtr(Bars bars, int period)
    {
        if (bars.Count < period + 1)
            return 0m;

        decimal total = 0m;

        for (int i = bars.Count - period; i < bars.Count; i++)
        {
            var high = (decimal)bars.HighPrices[i];
            var low = (decimal)bars.LowPrices[i];
            var prevClose = (decimal)bars.ClosePrices[i - 1];

            var tr1 = high - low;
            var tr2 = Math.Abs(high - prevClose);
            var tr3 = Math.Abs(low - prevClose);

            var tr = Math.Max(tr1, Math.Max(tr2, tr3));

            total += tr;
        }

        return total / period;
    }
}