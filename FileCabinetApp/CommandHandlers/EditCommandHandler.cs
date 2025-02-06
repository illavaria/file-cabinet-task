using System.Globalization;
using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for edit operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class EditCommandHandler(IFileCabinetService fileCabinetService, Func<FileCabinetRecordsParameters> inputParameters)
    : ServiceCommandHandleBase(fileCabinetService, "edit")
{
    private Func<FileCabinetRecordsParameters> inputParameters = inputParameters ?? throw new ArgumentNullException(nameof(inputParameters));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        if (!int.TryParse(parameters, CultureInfo.InvariantCulture, out var recordId))
        {
            Console.WriteLine("Pass one number as a record id");
            return;
        }

        if (this.fileCabinetService.FindById(recordId) is null)
        {
            Console.WriteLine($"Record #{recordId} is not found.");
            return;
        }

        while (true)
        {
            try
            {
                var recordParameters = this.inputParameters();
                this.fileCabinetService.EditRecord(recordId, recordParameters);
                Console.WriteLine($"Record #{recordId} is updated.");
                return;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press esc to cancel edit or any other key to try again");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(" Update canceled");
                    return;
                }
            }
        }
    }
}