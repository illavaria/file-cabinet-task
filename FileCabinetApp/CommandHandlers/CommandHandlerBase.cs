namespace FileCabinetApp;

public abstract class CommandHandlerBase : ICommandHandler
{
    public ICommandHandler NextHandler { get; private set; }

    public void SetNext(ICommandHandler commandHandler) => this.NextHandler = commandHandler;

    public abstract void Handle(AppCommandRequest commandRequest);
}