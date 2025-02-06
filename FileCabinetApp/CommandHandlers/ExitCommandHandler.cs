namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for exit operation.
/// </summary>
public class ExitCommandHandler(Action<bool> changeState)
    : CommandHandlerBase("exit")
{
    private readonly Action<bool> changeState = changeState ?? throw new ArgumentNullException(nameof(changeState));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        Console.WriteLine("Exiting an application...");
        this.changeState(false);
    }
}