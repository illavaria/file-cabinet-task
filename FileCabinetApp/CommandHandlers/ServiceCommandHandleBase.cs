namespace FileCabinetApp;

public abstract class ServiceCommandHandleBase(IFileCabinetService fileCabinetService) : CommandHandlerBase
{
    protected IFileCabinetService fileCabinetService = fileCabinetService;
}