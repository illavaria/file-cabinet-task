using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for purge operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class PurgeCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "purge")
{
    private new readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command doesn't take any arguments");
            return;
        }

        var recordBefore = this.fileCabinetService.GetNumberOfAllRecords();
        try
        {
            this.fileCabinetService.Purge();
            Console.WriteLine(
                $"Data file processing is completed: {recordBefore - this.fileCabinetService.GetNumberOfAllRecords()} of {recordBefore} records were purged.");
        }
        catch (NotSupportedException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}