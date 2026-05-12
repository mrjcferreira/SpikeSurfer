namespace SpikeSurfer.Models;

public sealed record MarketContext(
    string Symbol,
    string Session,
    bool LondonOpen,
    bool NewYorkOpen,
    bool AsiaSession,
    bool HighImpactNews
);