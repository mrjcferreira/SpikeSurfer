namespace SpikeSurfer.Models;

public sealed record WaveState(
    double ExpansionStrength,
    double WaveEnergy,
    double EnergySlope,
    bool FailureDetected,
    bool BullishPressure,
    bool BearishPressure,
    string WavePhase
);