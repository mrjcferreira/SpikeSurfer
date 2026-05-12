namespace SpikeSurfer.Models;

public sealed record AiValidation(
    bool Approved,
    double Confidence,
    string Reason
);