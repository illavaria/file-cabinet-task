namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for help operation.
/// </summary>
public class HelpCommandHandler() : CommandHandlerBase("help")
{
    private const int CommandHelpIndex = 0;
    private const int DescriptionHelpIndex = 1;
    private const int ExplanationHelpIndex = 2;

    private static string[][] helpMessages =
    [
        ["create", "The 'create' command creates a new record.", "Syntax: create"],
        ["delete", "The 'delete' command deletes a record that satisfies the condition.", "Syntax: delete where <condition>"],
        ["edit", "The 'edit' command edits record's data.", "Syntax: edit <id>"],
        ["exit", "The 'exit' command exits the application.", "Syntax: exit"],
        ["export", "The 'export' command exports records to a file.", "Syntax: export <format> <filename>"],
        ["find", "The 'find' command prints records with the needed value.", "Syntax: find <field> <value>"],
        ["help", "The 'help' command prints the help screen.", "Syntax: help [command]"],
        ["import", "The 'import' command imports records from file.", "Syntax: import <format> <filename>"],
        ["insert", "The 'insert' command creates a new record with specified id.", "Syntax: insert (<fields>) values (<values>)"],
        ["list", "The 'list' command prints information about all records.", "Syntax: list"],
        ["purge", "The 'purge' command removes deleted records.", "Syntax: purge"],
        ["remove", "The 'remove' command removes the record by its id.", "Syntax: remove <id>"],
        ["select", "The 'select' command prints selected fields for records that satisfy the condition", "Syntax: select <field1>, <field2> where <field3> = '<value1>' and <field4> = '<value2>'"],
        ["stat", "The 'stat' command prints the statistics of records.", "Syntax: stat"],
        ["update", "The 'update' command updates some fields of records that satisfy the condition.", "Syntax: update set <field1> = <value1>, <field2> = <value2> where <field3> = '<value3>' and <field4> = '<values4>'"]
    ];

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
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