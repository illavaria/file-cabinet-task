namespace FileCabinetApp;

/// <inheritdoc />
public class ExitCommandHandler(Action<bool> changeState): CommandHandlerBase
{
    private const string CommandName = "exit";

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Exit(commandRequest.Parameters);
    }
    
    private void Exit(string parameters)
    {
        Console.WriteLine("Exiting an application...");
        changeState(false);
    }
}