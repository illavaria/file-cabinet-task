namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for help operation.
/// </summary>
public class HelpCommandHandler : CommandHandlerBase
{
    private const int CommandHelpIndex = 0;
    private const int DescriptionHelpIndex = 1;
    private const int ExplanationHelpIndex = 2;

    private static string[][] helpMessages =
    [
        ["help", "prints the help screen", "The 'help' command prints the help screen."],
        ["exit", "exits the application", "The 'exit' command exits the application."],
        ["stat", "prints the statistics of records", "The 'stat' command prints the statistics of records"],
        ["create", "creates a new record", "The 'create' command creates a new record"],
        ["list", "prints all records", "The 'list' command prints prints information about all records."],
        ["edit", "edits record's data", "The 'edit' command edits record's data."],
        ["find", "finds records", "The 'find' command prints records with the needed value."],
        ["export", "exports records", "The 'export' command exports records to a file."],
        ["import", "imports records", "The 'import' command imports records from file."],
        ["remove", "removes the record", "The 'remove' command removes the record by its id"],
        ["purge", "removes deleted records", "The 'purge' command removes deleted records"]
    ];

    public HelpCommandHandler()
        : base("help")
    {
    }

    protected override void HandleCore(string parameters)
    {
        if (!string.IsNullOrEmpty(parameters))
        {
            var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
            Console.WriteLine(index >= 0
                ? helpMessages[index][ExplanationHelpIndex]
                : $"There is no explanation for '{parameters}' command.");
        }
        else
        {
            Console.WriteLine("Available commands:");

            foreach (var helpMessage in helpMessages)
            {
                Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
            }
        }

        Console.WriteLine();
    }
}