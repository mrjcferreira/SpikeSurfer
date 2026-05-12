# SpikeSurfer — Market State Bible

Every field in MarketReadResult exists for a reason. This document defines what each state means, when it fires, and how it influences the final ExecutionIntent.

---

## MarketContext

The session and symbol awareness layer.

| Field           | Type   | Meaning                                              |
|-----------------|--------|------------------------------------------------------|
| Symbol          | string | Instrument being traded (e.g. "XAUUSD")              |
| Session         | string | Current session description / regime reason           |
| LondonOpen      | bool   | London session is active (07:00–16:00 UTC)            |
| NewYorkOpen     | bool   | New York session is active (12:00–21:00 UTC)          |
| AsiaSession     | bool   | Asia session is active (00:00–07:00 UTC)              |
| HighImpactNews  | bool   | High-impact news event within window                  |

**Rules:** Avoid new entries during AsiaSession unless regime is Spike. HighImpactNews blocks all new entries — exit-only mode.

---

## WaveState

Captures the energy and momentum of the current price wave.

| Field             | Type   | Meaning                                           |
|-------------------|--------|---------------------------------------------------|
| ExpansionStrength | double | How strong the current expansion move is (0–100)  |
| WaveEnergy        | double | Remaining energy in the wave (0–100)              |
| EnergySlope       | double | Rate of energy change (positive = accelerating)   |
| FailureDetected   | bool   | Wave failed to continue (reversal signal)         |
| BullishPressure   | bool   | Dominant buying pressure detected                 |
| BearishPressure   | bool   | Dominant selling pressure detected                |
| WavePhase         | string | Current phase: BOOT, EXPANSION, EXHAUSTION, DEAD  |

**Rules:** FailureDetected + ExhaustionDominant = high-confidence reversal setup. WavePhase "BOOT" means insufficient data — no trading.

---

## StructureState

Multi-timeframe trend and structure analysis.

| Field              | Type   | Meaning                                            |
|--------------------|--------|----------------------------------------------------|
| H1TrendConsistency | int    | H1 trend consistency score (higher = cleaner)      |
| H4TrendConsistency | int    | H4 trend consistency score                         |
| H1Structure        | string | H1 market structure label (e.g. "HH-HL", "NONE")  |
| H4Structure        | string | H4 market structure label                          |
| H1Bullish          | bool   | H1 is in bullish structure                         |
| H1Bearish          | bool   | H1 is in bearish structure                         |
| H4Bullish          | bool   | H4 is in bullish structure                         |
| H4Bearish          | bool   | H4 is in bearish structure                         |
| AllowsShort        | bool   | Structure permits short entries                    |
| AllowsLong         | bool   | Structure permits long entries                     |
| ContextScore       | double | Overall structural alignment score (0–100)         |

**Rules:** Never trade against both H1 and H4. AllowsLong/AllowsShort are the final structural gates — if false, the intent is clamped to Watch regardless of engine output.

---

## ExhaustionState

Detects when a move has overextended and is likely to reverse.

| Field                 | Type   | Meaning                                              |
|-----------------------|--------|------------------------------------------------------|
| ExhaustionScore       | double | Overall exhaustion level (0–100)                     |
| ExpansionStrength     | double | Strength of the preceding expansion                  |
| ExhaustionDominant    | bool   | Exhaustion signals outweigh continuation signals     |
| MomentumDecelerating  | bool   | Momentum is slowing down                             |
| FailureDetected       | bool   | A failed continuation attempt was detected           |
| WickRejection         | bool   | Long wick rejection at extreme (trap candle)         |
| DirtyTopOrBottom      | bool   | Messy price action at extreme (indecision)           |
| AgainstStrongStructure| bool   | Move pushed into a strong opposing structure level   |
| ExhaustionType        | string | Classification: "NONE", "SOFT", "HARD", "EXTREME"   |

**Rules:** HARD or EXTREME exhaustion with WickRejection = prime reversal territory. DirtyTopOrBottom reduces confidence — wait for cleaner confirmation.

---

## RiskState

Pre-trade risk assessment.

| Field              | Type    | Meaning                                          |
|--------------------|---------|--------------------------------------------------|
| RiskApproved       | bool    | Risk check passed — trade is permitted           |
| RiskScore          | double  | Overall risk score (lower = safer)               |
| SuggestedStopLoss  | decimal | Recommended SL price                             |
| SuggestedTakeProfit| decimal | Recommended TP price                             |

**Rules:** If RiskApproved is false, intent must be clamped to Watch. SL/TP are suggestions — the Execution layer may adjust based on ATR.

---

## AiValidation

Optional AI layer for cross-checking the engine's decision.

| Field      | Type   | Meaning                                          |
|------------|--------|--------------------------------------------------|
| Approved   | bool   | AI agrees with the proposed trade                |
| Confidence | double | AI confidence in its assessment (0–100)          |
| Reason     | string | AI explanation of its decision                   |

**Rules:** AI is advisory, not authoritative. If AI disapproves but engine confidence is > 85, the trade proceeds with a flag. If both disagree, intent becomes Watch.

---

## ExecutionIntent

The final output — what the system should do.

| Value        | Meaning                                  |
|--------------|------------------------------------------|
| Ignore       | No setup, do nothing                     |
| Watch        | Setup forming, monitor but don't execute |
| ExecuteLong  | Open long position                       |
| ExecuteShort | Open short position                      |

**Priority chain:** RegimeEngine → StrategyEngine → StructureState gates → RiskState gates → AiValidation check → Final ExecutionIntent.

---

## Decision Matrix

```
Regime = NoTrade           → Ignore (always)
RiskApproved = false       → Watch (always)
HighImpactNews = true      → Watch (always)
AllowsLong = false         → clamp ExecuteLong → Watch
AllowsShort = false        → clamp ExecuteShort → Watch
AI.Approved = false
  AND engine confidence < 85 → Watch
Otherwise                  → trust engine output
```
