namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for exit operation.
/// </summary>
public class ExitCommandHandler : CommandHandlerBase
{
    private readonly Action<bool> changeState;

    public ExitCommandHandler(Action<bool> changeState)
        : base("exit")
    {
        this.changeState = changeState;
    }

    protected override void HandleCore(string parameters)
    {
        Console.WriteLine("Exiting an application...");
        this.changeState(false);
    }
}