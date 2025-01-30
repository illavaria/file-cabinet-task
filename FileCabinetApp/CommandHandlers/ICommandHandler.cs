namespace FileCabinetApp;

/// <summary>
/// Interface representing command handler.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Sets the next command.
    /// </summary>
    /// <param name="commandHandler">Command to be set next.</param>
    public void SetNext(ICommandHandler commandHandler);

    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="commandRequest">Command request to handle.</param>
    public void Handle(AppCommandRequest commandRequest);
}