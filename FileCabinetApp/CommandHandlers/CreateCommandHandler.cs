namespace FileCabinetApp;

public class CreateCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "create";

    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Create(commandRequest.Parameters);
    }

    private void Create(string parameters)
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
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press esc to cancel creation or any other key to try again");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(" Creation canceled");
                    return;
                }
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