using System.Globalization;

namespace FileCabinetApp;

public class RemoveCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "remove";

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Remove(commandRequest.Parameters);
    }

    private void Remove(string parameters)
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