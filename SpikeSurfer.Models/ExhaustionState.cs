namespace SpikeSurfer.Models;

public sealed record ExhaustionState(
    double ExhaustionScore,
    double ExpansionStrength,
    bool ExhaustionDominant,
    bool MomentumDecelerating,
    bool FailureDetected,
    bool WickRejection,
    bool DirtyTopOrBottom,
    bool AgainstStrongStructure,
    string ExhaustionType
);