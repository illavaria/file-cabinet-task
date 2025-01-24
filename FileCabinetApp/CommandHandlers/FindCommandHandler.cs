using System.Collections.ObjectModel;

namespace FileCabinetApp;

public class FindCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "find";
    private (string, Func<string, ReadOnlyCollection<FileCabinetRecord>>)[] findParams =
    [
        new ("firstName", fileCabinetService.FindByFirstName),
        new ("lastName", fileCabinetService.FindByLastName),
        new ("dateOfBirth", fileCabinetService.FindByDateOfBirth)
    ];

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Find(commandRequest.Parameters);
    }

    private void Find(string parameters)
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

        var results = this.findParams[index].Item2(par[1].Trim('"'));
        foreach (var record in results)
        {
            Console.WriteLine(record.ToString());
        }
    }
}