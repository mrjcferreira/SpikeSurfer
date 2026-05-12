namespace SpikeSurfer.Models;

public sealed record MarketReadResult(
    MarketContext Context,
    WaveState Wave,
    StructureState Structure,
    ExhaustionState Exhaustion,
    RiskState Risk,
    AiValidation? Ai,
    ExecutionIntent Intent
);