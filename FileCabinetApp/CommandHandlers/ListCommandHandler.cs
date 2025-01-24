namespace FileCabinetApp;

public class ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer) 
    : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "list";

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.List(commandRequest.Parameters);
    }

    private void List(string parameters)
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