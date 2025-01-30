namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for unknown operation.
/// </summary>
public class UnknownCommandHandler : CommandHandlerBase
{
    public UnknownCommandHandler(string commandName)
        :base(commandName)
    {
    }

    protected override void HandleCore(string parameters)
    {
        Console.WriteLine("There is no such command.");
        Console.WriteLine();
    }
}