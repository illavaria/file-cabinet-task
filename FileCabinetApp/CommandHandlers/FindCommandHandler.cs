using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for find operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
    : ServiceCommandHandleBase(fileCabinetService, "find")
{
    private new readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
    private readonly Action<IEnumerable<FileCabinetRecord>> printer = printer ?? throw new ArgumentNullException(nameof(printer));

    private readonly (string, Func<string, IEnumerable<FileCabinetRecord>>)[] findParams =
    [
        new ("firstName", fileCabinetService.FindByFirstName),
        new ("lastName", fileCabinetService.FindByLastName),
        new ("dateOfBirth", fileCabinetService.FindByDateOfBirth)
    ];

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("Pass field's name and value as the parameters to the command");
            return;
        }

        var par = parameters.Split(' ', 2);
        if (par.Length < 2)
        {
            Console.WriteLine("Command takes 2 parameters: field's name and value");
            return;
        }

        var index = Array.FindIndex(this.findParams, tuple => string.Equals(par[0], tuple.Item1, StringComparison.OrdinalIgnoreCase));
        if (index == -1)
        {
            Console.WriteLine("Wrong field name");
            return;
        }

        var results = this.findParams[index].Item2(par[1].Trim('\''));
        this.printer(results);
    }
}