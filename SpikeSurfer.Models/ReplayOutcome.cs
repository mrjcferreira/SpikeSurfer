namespace SpikeSurfer.Models;

public sealed record ReplayOutcome(
    double MaxFavourablePips,
    double MaxAdversePips,
    string FinalResult,
    string Notes
);