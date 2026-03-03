using LightweightDataManipulator.Models;
using LightweightDataManipulator.Services;

var pipeline = new DataManipulationPipeline();
var options = AppOptions.Parse(args);

if (options.ShowHelp)
{
    AppOptions.PrintHelp();
    return;
}

await pipeline.ProcessAsync(options, CancellationToken.None);
