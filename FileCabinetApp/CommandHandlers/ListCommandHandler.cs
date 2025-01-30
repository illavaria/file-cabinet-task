namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for list operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer) 
    : ServiceCommandHandleBase(fileCabinetService, "list")
{
    /// <inheritdoc/>
    protected override void HandleCore(string parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command doesn't take any arguments");
            return;
        }

        var records = this.fileCabinetService.GetRecords();
        printer(records);
    }
}