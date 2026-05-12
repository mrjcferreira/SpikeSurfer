# SpikeSurfer — Roadmap

## Phase 1: Foundation ✅

The skeleton is in place. Models are defined, core data flow works, build passes.

- [x] Solution structure with modular projects
- [x] Immutable record models (MarketSnapshot, MarketReadResult, all state records)
- [x] SpikeSurferEngine orchestrator with Read() pipeline
- [x] RegimeEngine (stub — hardcoded for testing)
- [x] StrategyEngine (regime → strategy switch)
- [x] OpenRangeEngine (real logic: range detection, false breaks)
- [x] SecondWaveReaperEngine (second wave trap detection)
- [x] CTraderSnapshotBuilder (stub — empty data)
- [x] Console app runner (Program.cs)
- [x] ExecutionIntent enum (Ignore, Watch, ExecuteLong, ExecuteShort)

## Phase 2: Real Market Data

Connect to live cTrader data so engines receive real candles.

- [ ] CTraderSnapshotBuilder reads real candle history from cTrader API
- [ ] Populate M1, M5, M15, H1, H4 with actual XAUUSD data
- [ ] Bid/Ask/Spread from live feed
- [ ] Session detection (London, New York, Asia) in MarketContext
- [ ] High-impact news calendar integration

## Phase 3: Engine Intelligence

Implement the empty engines with real analysis logic.

- [ ] **RegimeEngine** — classify market from candle data (replace hardcoded stub)
  - ATR-based volatility analysis
  - Session-aware regime detection
  - Trend vs range classification
- [ ] **WaveClassifier** — detect wave phases (Expansion → Exhaustion → Dead)
  - Energy calculation from price momentum
  - Slope analysis for deceleration detection
- [ ] **AbsorptionEngine** — detect volume absorption at key levels
- [ ] **CompressionEngine** — identify compression/consolidation before breakouts
- [ ] **ContinuationEngine** — validate continuation setups after pullbacks
- [ ] **ReleaseEngine** — detect energy release after compression

## Phase 4: Risk & Execution

Gate trades properly and manage positions.

- [ ] **RunnerGuard** — risk management layer
  - Max daily loss limit
  - Max concurrent positions
  - Risk-per-trade calculation (% of account)
  - ATR-based SL/TP calculation
- [ ] **Execution layer** — translate ExecutionIntent into cTrader orders
  - Market orders for ExecuteLong/ExecuteShort
  - Break-even logic after X pips profit
  - Trailing stop management
  - Daily close: flatten all positions before market close

## Phase 5: AI Validation

Add Claude API as a second opinion layer.

- [ ] HTTP client for Anthropic API
- [ ] Prompt engineering: send MarketReadResult as context
- [ ] Parse AI response into AiValidation record
- [ ] Decision matrix: engine + AI agreement check
- [ ] Confidence threshold tuning
- [ ] Rate limiting and fallback (AI unavailable → proceed without)

## Phase 6: Memory & Telemetry

Learn from past trades and maintain audit trail.

- [ ] **WaveMemoryService** — persist wave patterns and outcomes
  - Store wave fingerprints with trade results
  - Pattern recognition from historical waves
- [ ] **WaveEvent / AuditResult** — telemetry pipeline
  - Log every MarketReadResult with timestamp
  - Track intent vs actual execution vs outcome
  - Win rate per regime, per strategy, per session
- [ ] Dashboard or log viewer for post-session review

## Phase 7: Hardening

Production readiness.

- [ ] Add .gitignore (exclude bin/, obj/, .DS_Store)
- [ ] Add missing projects to SpikeSurfer.slnx
- [ ] Remove Class1.cs placeholder files
- [ ] Unit tests for each engine
- [ ] Integration test: full pipeline with mock snapshot
- [ ] Upgrade from .NET 6 to .NET 8+ (LTS)
- [ ] Configuration via appsettings.json or environment variables
- [ ] Secrets management (API keys not in source)

## Phase 8: Live Trading

Deploy and monitor.

- [ ] cBot wrapper for cTrader deployment
- [ ] Real-time loop: snapshot → read → execute → log
- [ ] Paper trading mode (log intents without executing)
- [ ] Alerting (Telegram/Discord notifications on trade signals)
- [ ] Performance monitoring and auto-shutdown on anomalies
