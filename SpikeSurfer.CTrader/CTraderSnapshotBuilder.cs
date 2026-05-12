using SpikeSurfer.Models;
using System.Collections.Generic;

namespace SpikeSurfer.CTrader;

public static class CTraderSnapshotBuilder
{
    public static MarketSnapshot Build()
    {
        var now = DateTime.UtcNow;

        return new MarketSnapshot(
            now,
            0m,
            0m,
            0m,
            new List<Candle>(),
            new List<Candle>(),
            new List<Candle>(),
            new List<Candle>(),
            new List<Candle>()
        );
    }
}