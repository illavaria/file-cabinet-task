namespace FileCabinetApp;

/// <summary>
/// Class represents command handler for create operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class CreateCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService, "create")
{
    /// <inheritdoc/>
    protected override void HandleCore(string parameters)
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
                Program.InputParameters(out var firstName, out var lastName, out var dateOfBirth, out var numberOfChildren, out var yearIncome, out var gender);
                var recordId = this.fileCabinetService.CreateRecord(new FileCabinetRecordsParameters
                {
                    FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth,
                    NumberOfChildren = numberOfChildren, YearIncome = yearIncome, Gender = gender,
                });
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