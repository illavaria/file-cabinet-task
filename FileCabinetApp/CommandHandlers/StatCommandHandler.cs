using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for stat operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class StatCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "stat")
{
    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command doesn't take any arguments");
            return;
        }

        Console.WriteLine($"Number of all records: {this.fileCabinetService.GetNumberOfAllRecords()}.");
        Console.WriteLine($"Number of deleted records: {this.fileCabinetService.GetNumberOfDeletedRecords()}.");
    }
}