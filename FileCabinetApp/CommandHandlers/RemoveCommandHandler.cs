using System.Globalization;

namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for remove operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class RemoveCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService, "remove")
{
    /// <inheritdoc/>
    protected override void HandleCore(string parameters)
    {
        if (!int.TryParse(parameters, CultureInfo.InvariantCulture, out var recordId))
        {
            Console.WriteLine("Pass one number as a record id");
            return;
        }

        try
        {
            this.fileCabinetService.RemoveRecord(recordId);
            Console.WriteLine($"Record {recordId} is removed.");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}