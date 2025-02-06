namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for unknown operation.
/// </summary>
public class UnknownCommandHandler(string commandName)
    : CommandHandlerBase(commandName)
{
    private static readonly List<string> Commands =
    [
        "create", "delete", "edit", "exit", "export", "find", "help", "import", "insert", "list", "purge", "remove", "select", "stat", "update"
    ];

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        Console.WriteLine($"There is no '{this.commandName}' command. Use 'help' command to see available commands.");

        var suggestions = GetClosestCommands(this.commandName);
        if (suggestions.Count > 0)
        {
            Console.WriteLine("The most similar commands are:");
            foreach (var suggestion in suggestions)
            {
                Console.WriteLine($"- {suggestion}");
            }
        }
    }

    private static List<string> GetClosestCommands(string input)
    {
        return Commands.Select(cmd => new { Command = cmd, Distance = GetLevenshteinDistance(input, cmd) })
            .OrderBy(x => x.Distance)
            .Where(x => x.Distance <= 2)
            .Select(x => x.Command)
            .ToList();
    }

    private static int GetLevenshteinDistance(string source, string target)
    {
        if (source.Length == 0)
        {
            return target.Length;
        }

        if (target.Length == 0)
        {
            return source.Length;
        }

        var dp = new int[source.Length + 1, target.Length + 1];

        for (var i = 0; i <= source.Length; i++)
        {
            dp[i, 0] = i;
        }

        for (var j = 0; j <= target.Length; j++)
        {
            dp[0, j] = j;
        }

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
            }
        }

        return dp[source.Length, target.Length];
    }
}