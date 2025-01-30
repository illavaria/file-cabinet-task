namespace FileCabinetApp;

/// <summary>
/// Abstract class representing command handler base that uses file cabinet service.
/// </summary>
public abstract class ServiceCommandHandleBase : CommandHandlerBase
{
    protected readonly IFileCabinetService fileCabinetService;

    protected ServiceCommandHandleBase(IFileCabinetService fileCabinetService, string commandName)
        : base(commandName)
    {
        this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
    }
}