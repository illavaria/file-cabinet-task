using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for list operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
    : ServiceCommandHandleBase(fileCabinetService, "list")
{
    private new readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
    private readonly Action<IEnumerable<FileCabinetRecord>> printer = printer ?? throw new ArgumentNullException(nameof(printer));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command doesn't take any arguments");
            return;
        }

        var records = this.fileCabinetService.GetRecords();
        this.printer(records);
    }
}