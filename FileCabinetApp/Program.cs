using System.Globalization;
using System.Runtime.CompilerServices;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Illaria Samal";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new ();

        private static Tuple<string, Action<string>>[] commands =
        [
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find)
        ];

        private static Tuple<string, Func<string, FileCabinetRecord[]>>[] findParams =
        [
            new Tuple<string, Func<string, FileCabinetRecord[]>>("firstName", fileCabinetService.FindByFirstName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("lastName", fileCabinetService.FindByLastName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("dateOfBirth", fileCabinetService.FindByDateOfBirth)
        ];

        private static string[][] helpMessages =
        [
            ["help", "prints the help screen", "The 'help' command prints the help screen."],
            ["exit", "exits the application", "The 'exit' command exits the application."],
            ["stat", "prints the statistics of records", "The 'stat' command prints the statistics of records"],
            ["create", "creates a new record", "The 'create' command creates a new record"],
            ["list", "prints all records", "The 'list' command prints prints information about all records."],
            ["edit", "edits record's data", "The 'edit' command edits record's data."],
            ["find", "finds records", "The 'find' command prints records with the needed value"]
        ];

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : [string.Empty, string.Empty];
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(index >= 0
                    ? helpMessages[index][ExplanationHelpIndex]
                    : $"There is no explanation for '{parameters}' command.");
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            while (true)
            {
                try
                {
                    InputParameters(out var firstName, out var lastName, out var dateOfBirth, out var numberOfChildren, out var yearIncome, out var gender);
                    var recordId = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, numberOfChildren, yearIncome, gender);
                    Console.WriteLine($"Record #{recordId} is created.");
                    return;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press esc to cancel creation or any other key to try again");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Creation canceled");
                        return;
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press esc to cancel creation or any other key to try again");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Creation canceled");
                        return;
                    }
                }
            }
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        private static void Edit(string parameters)
        {
            int recordId;
            if (!int.TryParse(parameters, CultureInfo.InvariantCulture, out recordId))
            {
                Console.WriteLine("Pass one number as a record id");
                return;
            }

            if (fileCabinetService.FindById(recordId) is null)
            {
                Console.WriteLine($"Record #{recordId} is not found.");
                return;
            }

            while (true)
            {
                try
                {
                    InputParameters(out var firstName, out var lastName, out var dateOfBirth, out var numberOfChildren, out var yearIncome, out var gender);

                    fileCabinetService.EditRecord(recordId, firstName, lastName, dateOfBirth, numberOfChildren, yearIncome, gender);
                    Console.WriteLine($"Record #{recordId} is updated.");
                    return;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press esc to cancel creation or any other key to try again");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Update canceled");
                        return;
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press esc to cancel creation or any other key to try again");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Update canceled");
                        return;
                    }
                }
            }
        }

        private static void Find(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Pass field's name and value as the parameters to the command");
                return;
            }

            var par = parameters.Split(' ', 2);
            if (par.Length < 2)
            {
                Console.WriteLine("Command takes 2 parameters: field's name and value");
                return;
            }

            var index = Array.FindIndex(findParams,
                tuple => string.Equals(par[0], tuple.Item1, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                Console.WriteLine("Wrong field name");
                return;
            }

            var results = findParams[index].Item2(par[1].Trim('"'));
            foreach (var record in results)
            {
                Console.WriteLine(record.ToString());
            }
        }

        private static void InputParameters(out string? firstName, out string? lastName, out DateTime dateOfBirth, out short numberOfChildren, out decimal yearIncome, out char gender)
        {
            Console.Write("First name: ");
            firstName = Console.ReadLine();
            Console.Write("Last name: ");
            lastName = Console.ReadLine();

            Console.Write("Date of birth: ");
            var input = Console.ReadLine();
            DateTime.TryParse(input, CultureInfo.InvariantCulture, out dateOfBirth);

            Console.Write("Number of children: ");
            input = Console.ReadLine();
            short.TryParse(input, CultureInfo.InvariantCulture, out numberOfChildren);

            Console.Write("Year income: ");
            input = Console.ReadLine();
            decimal.TryParse(input, CultureInfo.InvariantCulture, out yearIncome);

            Console.Write("Gender (M, F, N): ");
            input = Console.ReadLine();
            input = input?.ToUpper(CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(input) || input.Length > 1)
            {
                throw new ArgumentException("Gender can't have more than 1 symbol");
            }

            gender = input[0];
        }
    }
}