namespace FileCabinetApp;

public class StatCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "stat";

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Stat(commandRequest.Parameters);
    }

    private void Stat(string parameters)
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