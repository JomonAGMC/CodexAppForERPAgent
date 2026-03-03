namespace LightweightDataManipulator.Models;

public sealed record AppOptions(
    string InputPath,
    string OutputPath,
    string GroupByColumn,
    string AggregateColumn,
    bool ShowHelp = false)
{
    public static AppOptions Parse(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
        {
            return new AppOptions(string.Empty, string.Empty, string.Empty, string.Empty, ShowHelp: true);
        }

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for argument '{args[i]}'");
            }

            map[args[i]] = args[i + 1];
        }

        return new AppOptions(
            InputPath: ReadRequired(map, "--input"),
            OutputPath: ReadRequired(map, "--output"),
            GroupByColumn: ReadRequired(map, "--group-by"),
            AggregateColumn: ReadRequired(map, "--sum"));
    }

    public static void PrintHelp()
    {
        Console.WriteLine("LightweightDataManipulator");
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run -- --input <csvPath> --output <csvPath> --group-by <column> --sum <column>");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  dotnet run -- --input big-data.csv --output result.csv --group-by Region --sum Revenue");
    }

    private static string ReadRequired(IReadOnlyDictionary<string, string> map, string key)
    {
        if (!map.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Missing required argument: {key}");
        }

        return value;
    }
}
