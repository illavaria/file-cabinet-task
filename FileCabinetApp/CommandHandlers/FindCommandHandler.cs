using System.Collections;
using System.Collections.ObjectModel;

namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for find operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
    : ServiceCommandHandleBase(fileCabinetService, "find")
{
    private (string, Func<string, IEnumerable<FileCabinetRecord>>)[] findParams =
    [
        new ("firstName", fileCabinetService.FindByFirstName),
        new ("lastName", fileCabinetService.FindByLastName),
        new ("dateOfBirth", fileCabinetService.FindByDateOfBirth)
    ];

    /// <inheritdoc/>
    protected override void HandleCore(string parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("Pass field's name and value as the parameters to the command");
            return;
        }

        var par = parameters.Split(" = ", 2);
        if (par.Length < 2)
        {
            Console.WriteLine("Command takes 2 parameters: field's name and value");
            return;
        }

        var conditions = new Dictionary<string, string> { { par[0], par[1].Trim('\'') } };
        IEnumerable<FileCabinetRecord> results;

        var index = Array.FindIndex(this.findParams, tuple => string.Equals(par[0], tuple.Item1, StringComparison.OrdinalIgnoreCase));
        if (index == -1)
        {
            try
            {
                results = this.fileCabinetService.Find(conditions);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
        else
        {
            results = this.findParams[index].Item2(par[1].Trim('"'));
        }

        printer(results);
    }
}