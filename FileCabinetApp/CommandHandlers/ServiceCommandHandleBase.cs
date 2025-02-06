using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Abstract class representing command handler base that uses file cabinet service.
/// </summary>
public abstract class ServiceCommandHandleBase(IFileCabinetService fileCabinetService, string commandName)
    : CommandHandlerBase(commandName)
{
    /// <summary>
    /// File cabinet service that command work with.
    /// </summary>
    protected readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
}