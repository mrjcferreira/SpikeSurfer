namespace SpikeSurfer.Models;

public sealed record MarketSnapshot(
    DateTime Timestamp,
    decimal Bid,
    decimal Ask,
    decimal Spread,
    IReadOnlyList<Candle> M1,
    IReadOnlyList<Candle> M5,
    IReadOnlyList<Candle> M15,
    IReadOnlyList<Candle> H1,
    IReadOnlyList<Candle> H4
);