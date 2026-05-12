                                                        # SpikeSurfer — Architecture

## Overview

SpikeSurfer is a modular XAUUSD trading engine built in C# / .NET 6. It reads multi-timeframe market data, classifies the current regime, selects the optimal strategy, and outputs a structured execution intent — all through immutable records and isolated engines.

## Solution Structure

```
SpikeSurfer.slnx
│
├── SpikeSurfer.Models        # Immutable records: the shared language
├── SpikeSurfer.Engines        # Independent analysis engines (stateless)
├── SpikeSurfer.Core           # Orchestrator: SpikeSurferEngine.Read()
├── SpikeSurfer.CTrader        # Platform adapter: builds MarketSnapshot from cTrader
├── SpikeSurfer.App            # Entry point / runner
│
├── SpikeSurfer.AI             # AI validation layer (Claude API integration)
├── SpikeSurfer.Classification # Market classification logic
├── SpikeSurfer.Config         # Configuration & parameters
├── SpikeSurfer.Execution      # Order execution & management
├── SpikeSurfer.Market         # Market data services
├── SpikeSurfer.Memory         # Wave memory / state persistence
├── SpikeSurfer.Risk           # Risk management (RunnerGuard)
├── SpikeSurfer.Telemetry      # Audit trail & wave events
└── SpikeSurfer.Tests          # Unit & integration tests
```

## Data Flow

```
cTrader Market Data
       │
       ▼
CTraderSnapshotBuilder.Build()
       │
       ▼
  MarketSnapshot (M1, M5, M15, H1, H4 candles + Bid/Ask/Spread)
       │
       ▼
SpikeSurferEngine.Read(snapshot)
       │
       ├──► RegimeEngine.Analyze()        → RegimeState (regime + confidence)
       │
       ├──► StrategyEngine.Select()       → StrategyState (strategy + intent)
       │
       ├──► OpenRangeEngine.Analyze()     → OpenRangeState (range breaks)
       │
       ├──► SecondWaveReaperEngine.Analyze() → StrategyState (trap detection)
       │
       └──► [Future: AI validation, risk check, exhaustion analysis]
              │
              ▼
       MarketReadResult
       (Context + Wave + Structure + Exhaustion + Risk + AI + Intent)
```

## Core Principles

**Immutable state** — All data flows through sealed records. No mutation, no side effects. Every engine receives a snapshot and returns a new state object.

**Engine isolation** — Each engine is stateless and testable in isolation. Engines never reference other engines directly; the orchestrator composes them.

**Regime-first decisions** — The RegimeEngine classifies the market first (Normal, OpenRange, Exhaustion, Spike, Reversal, NoTrade). The StrategyEngine then picks the right playbook for that regime.

**Multi-timeframe input** — MarketSnapshot carries M1 through H4 candles. Engines choose which timeframe to analyze based on their purpose.

## Dependency Graph

```
Models ◄── Engines ◄── Core ◄── App
                         ▲
Models ◄── CTrader ──────┘
```

Models has zero dependencies. Everything depends on Models. Core depends on Engines. App wires it all together.

## Market Regimes

| Regime     | Description                                  | Primary Strategy         |
|------------|----------------------------------------------|--------------------------|
| Normal     | No clear setup, observe                      | NORMAL_MARKET            |
| OpenRange  | London/NY open range active                  | OPEN_RANGE               |
| Exhaustion | Extended move losing momentum                | EXHAUSTION_WAVE_8000     |
| Spike      | Sharp directional move                       | SPIKE_SURF               |
| Reversal   | Structure failure after expansion            | REVERSAL_AFTER_FAILURE   |
| NoTrade    | Conditions unfit for trading                 | —                        |

## Key Records

- **MarketSnapshot** — Raw market data input (timestamp, bid/ask, candles per timeframe)
- **MarketReadResult** — Complete market analysis output (context, wave, structure, exhaustion, risk, AI, intent)
- **RegimeState** — Which regime + confidence + reason
- **StrategyState** — Which strategy + confidence + reason + execution intent
- **ExecutionIntent** — Final decision: `Ignore`, `Watch`, `ExecuteLong`, `ExecuteShort`
