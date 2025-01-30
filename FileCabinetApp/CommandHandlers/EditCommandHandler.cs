using System.Globalization;

namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for edit operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class EditCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService, "edit")
{
    /// <inheritdoc/>
    protected override void HandleCore(string parameters)
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
                Program.InputParameters(out var firstName, out var lastName, out var dateOfBirth, out var numberOfChildren, out var yearIncome, out var gender);
                this.fileCabinetService.EditRecord(recordId, new FileCabinetRecordsParameters
                {
                    FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth,
                    NumberOfChildren = numberOfChildren, YearIncome = yearIncome, Gender = gender,
                });
                Console.WriteLine($"Record #{recordId} is updated.");
                return;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press esc to cancel creation or any other key to try again");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(" Update canceled");
                    return;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press esc to cancel creation or any other key to try again");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(" Update canceled");
                    return;
                }
            }
        }
    }
}