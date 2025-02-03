namespace FileCabinetApp;

/// <summary>
/// Abstract class that represents command handler.
/// </summary>
public abstract class CommandHandlerBase : ICommandHandler
{
    protected readonly string commandName;

    protected CommandHandlerBase(string commandName)
    {
        this.commandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
    }

    /// <summary>
    /// Gets the next command handler.
    /// </summary>
    protected ICommandHandler NextHandler { get; private set; }

    /// <inheritdoc/>
    public void SetNext(ICommandHandler commandHandler) =>
        this.NextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));

    /// <inheritdoc/>
    public void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(this.commandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.HandleCore(commandRequest.Parameters);
    }

    protected abstract void HandleCore(string parameters);
}