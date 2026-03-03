using System.Globalization;
using LightweightDataManipulator.Models;

namespace LightweightDataManipulator.Services;

public sealed class DataManipulationPipeline
{
    public async Task ProcessAsync(AppOptions options, CancellationToken cancellationToken)
    {
        await using var input = new FileStream(options.InputPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1024 * 32, useAsync: true);
        using var reader = new StreamReader(input);

        var headerLine = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            throw new InvalidOperationException("Input file is empty.");
        }

        var headers = headerLine.Split(',');
        var groupIndex = Array.FindIndex(headers, h => string.Equals(h, options.GroupByColumn, StringComparison.OrdinalIgnoreCase));
        var aggregateIndex = Array.FindIndex(headers, h => string.Equals(h, options.AggregateColumn, StringComparison.OrdinalIgnoreCase));

        if (groupIndex < 0 || aggregateIndex < 0)
        {
            throw new InvalidOperationException("The provided group-by or sum column was not found in the CSV header.");
        }

        var summary = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            if (parts.Length <= Math.Max(groupIndex, aggregateIndex))
            {
                continue;
            }

            var key = parts[groupIndex];
            if (!double.TryParse(parts[aggregateIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                continue;
            }

            if (summary.TryGetValue(key, out var current))
            {
                summary[key] = current + value;
            }
            else
            {
                summary[key] = value;
            }
        }

        await using var output = new FileStream(options.OutputPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 1024 * 32, useAsync: true);
        await using var writer = new StreamWriter(output);
        await writer.WriteLineAsync($"{options.GroupByColumn},{options.AggregateColumn}Total");

        foreach (var entry in summary.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            await writer.WriteLineAsync($"{entry.Key},{entry.Value.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}
