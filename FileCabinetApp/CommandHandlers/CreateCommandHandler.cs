using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for create operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class CreateCommandHandler(IFileCabinetService fileCabinetService, Func<FileCabinetRecordsParameters> inputParameters)
    : ServiceCommandHandleBase(fileCabinetService, "create")
{
    private Func<FileCabinetRecordsParameters> inputParameters = inputParameters ?? throw new ArgumentNullException(nameof(inputParameters));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command doesn't take any arguments");
            return;
        }

        while (true)
        {
            try
            {
                var recordParameters = this.inputParameters();
                var recordId = this.fileCabinetService.CreateRecord(recordParameters);
                Console.WriteLine($"Record #{recordId} is created.");
                return;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press esc to cancel creation or any other key to try again");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(" Creation canceled");
                    return;
                }
            }
        }
    }
}