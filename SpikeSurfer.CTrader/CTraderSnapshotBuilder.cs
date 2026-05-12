using SpikeSurfer.Models;
using System.Collections.Generic;

namespace SpikeSurfer.CTrader;

public static class CTraderSnapshotBuilder
{
    public static MarketSnapshot Build(
        decimal currentPrice,
        decimal atr,
        decimal rsi,
        decimal ema21,
        List<Candle> m1,
        List<Candle> m5,
        List<Candle> m15,
        List<Candle> h1)
    {
        return new MarketSnapshot(
            TimestampUtc: DateTime.UtcNow,
            CurrentPrice: currentPrice,
            Atr: atr,
            Rsi: rsi,
            Ema21: ema21,
            M1: m1,
            M5: m5,
            M15: m15,
            H1: h1
        );
    }
}