namespace FileCabinetApp;

public class PurgeCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "purge";

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Purge(commandRequest.Parameters);
    }

    private void Purge(string parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command doesn't take any arguments");
            return;
        }

        var recordBefore = fileCabinetService.GetNumberOfAllRecords();
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