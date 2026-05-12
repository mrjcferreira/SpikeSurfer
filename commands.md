using cAlgo.API;
using SpikeSurfer.Core;
using SpikeSurfer.CTrader;
using SpikeSurfer.Models;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SpikeSurferBot : Robot
    {
        [Parameter("Volume", DefaultValue = 1000)]
        public int Volume { get; set; }

        [Parameter("Allow Execution", DefaultValue = false)]
        public bool AllowExecution { get; set; }

        private SpikeSurferEngine engine;

        protected override void OnStart()
        {
            engine = new SpikeSurferEngine();
            Print("SpikeSurfer connected");
        }

        protected override void OnBar()
        {
            var snapshot = CTraderSnapshotBuilder.Build();

            var result = engine.Read(snapshot);

            Print("Intent={0} | Confidence={1} | Reason={2}",
                result.Intent,
                result.Ai.Confidence,
                result.Ai.Reason);

            if (!AllowExecution)
            {
                Print("Execution disabled. Reading only.");
                return;
            }

            if (result.Intent == ExecutionIntent.ExecuteShort)
            {
                ExecuteMarketOrder(
                    TradeType.Sell,
                    SymbolName,
                    Volume,
                    "SPIKESURFER"
                );
            }

            if (result.Intent == ExecutionIntent.ExecuteLong)
            {
                ExecuteMarketOrder(
                    TradeType.Buy,
                    SymbolName,
                    Volume,
                    "SPIKESURFER"
                );
            }
        }
    }
}