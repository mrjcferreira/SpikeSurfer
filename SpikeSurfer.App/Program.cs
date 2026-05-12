using SpikeSurfer.Core;
using SpikeSurfer.CTrader;

var engine = new SpikeSurferEngine();

var snapshot = CTraderSnapshotBuilder.Build();

var result = engine.Read(snapshot);

Console.WriteLine(result);