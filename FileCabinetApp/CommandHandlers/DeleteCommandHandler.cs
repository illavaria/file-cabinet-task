using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for delete operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class DeleteCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "delete")
{
    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command takes parameters. Use: delete where <ColumnName> = '<Value>'");
            return;
        }

        var match = Regex.Match(parameters, @"where\s+(\w+)\s*=\s*'([^']+)'", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            Console.WriteLine("Invalid delete command format. Use: delete where <ColumnName> = '<Value>'");
            return;
        }

        var columnName = match.Groups[1].Value;
        var value = match.Groups[2].Value;
        if (columnName.Equals("id", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                this.fileCabinetService.RemoveRecord(int.Parse(value, CultureInfo.InvariantCulture));
                Console.WriteLine($"Record {value} is deleted.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            try
            {
                Dictionary<string, string> conditions = new() { { columnName, value } };
                var recordsToDelete = this.fileCabinetService.Find(conditions).ToList();
                if (recordsToDelete.Count == 0)
                {
                    Console.WriteLine("No records that satisfy the condition.");
                    return;
                }

                foreach (var record in recordsToDelete)
                {
                    try
                    {
                        this.fileCabinetService.RemoveRecord(record.Id);
                        Console.WriteLine($"Record {record.Id} is deleted.");
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}