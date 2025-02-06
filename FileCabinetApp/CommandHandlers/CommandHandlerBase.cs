namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Abstract class that represents command handler.
/// </summary>
public abstract class CommandHandlerBase(string commandName) : ICommandHandler
{
    protected readonly string commandName = commandName ?? throw new ArgumentNullException(nameof(commandName));

    /// <summary>
    /// Gets or sets the next command handler.
    /// </summary>
    private ICommandHandler? NextHandler { get; set; }

    /// <inheritdoc/>
    public void SetNext(ICommandHandler commandHandler) =>
        this.NextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));

    /// <inheritdoc/>
    public void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(this.commandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler?.Handle(commandRequest);
            return;
        }

        this.HandleCore(commandRequest.Parameters);
    }

    /// <summary>
    /// Abstract method representing handle logic.
    /// </summary>
    /// <param name="parameters">Command's parameters.</param>
    protected abstract void HandleCore(string? parameters);
}