using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Console client program.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Illaria Samal";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;

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

        private static Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] findParams;

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

        private static Tuple<string, Func<IRecordValidator>>[] validationParams =
        [
            new Tuple<string, Func<IRecordValidator>>("custom", () => new CustomValidator()),
            new Tuple<string, Func<IRecordValidator>>("default", () => new DefaultValidator())
        ];

        /// <summary>
        /// Main program body.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            var validationRule = "default";
            if (args?.Length > 0)
            {
                if (args[0].StartsWith("--validation-rules=", StringComparison.OrdinalIgnoreCase))
                {
                    validationRule = args[0]["--validation-rules=".Length..];
                }
                else if (string.Equals(args[0], "-v", StringComparison.OrdinalIgnoreCase))
                {
                    validationRule = args[1];
                }
            }

            var index = Array.FindIndex(validationParams, i => i.Item1.Equals(validationRule, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                Console.WriteLine("Unknown validation rule");
                return;
            }

            Program.fileCabinetService = new FileCabinetService(validationParams[index].Item2.Invoke());
            Program.validator = validationParams[index].Item2.Invoke();

            findParams =
            [
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstName", fileCabinetService.FindByFirstName),
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastName", fileCabinetService.FindByLastName),
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateOfBirth", fileCabinetService.FindByDateOfBirth)
            ];

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

                index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
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
                    var recordId = fileCabinetService.CreateRecord(new FileCabinetRecordsParameters
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
            if (!int.TryParse(parameters, CultureInfo.InvariantCulture, out var recordId))
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

                    fileCabinetService.EditRecord(recordId, new FileCabinetRecordsParameters
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

            var index = Array.FindIndex(findParams, tuple => string.Equals(par[0], tuple.Item1, StringComparison.OrdinalIgnoreCase));
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
            firstName = ReadInput(StringConverter, validator.FirstNameValidator);
            Console.Write("Last name: ");
            lastName = ReadInput(StringConverter, validator.LastNameValidator);

            Console.Write("Date of birth: ");
            dateOfBirth = ReadInput(DateConverter, validator.DateOfBirthValidator);

            Console.Write("Number of children: ");
            numberOfChildren = ReadInput(ShortConverter, validator.NumberOfChildrenValidator);

            Console.Write("Year income: ");
            yearIncome = ReadInput(DecimalConverter, validator.YearIncomeValidator);

            Console.Write("Gender (M, F, N): ");
            gender = ReadInput(CharConverter, validator.GenderValidator);
        }

        private static Tuple<bool, string, string> StringConverter(string input) =>
            new (true, "Correct parameter", input);

        private static Tuple<bool, string, DateTime> DateConverter(string input) =>
            DateTime.TryParse(input, CultureInfo.InvariantCulture, out var output)
                ? new Tuple<bool, string, DateTime>(true, "Correct parameter", output)
                : new Tuple<bool, string, DateTime>(false, "Not valid date format", output);

        private static Tuple<bool, string, short> ShortConverter(string input) =>
            short.TryParse(input, CultureInfo.InvariantCulture, out var output)
                ? new Tuple<bool, string, short>(true, "Correct parameter", output)
                : new Tuple<bool, string, short>(false, "Not valid number format", output);

        private static Tuple<bool, string, decimal> DecimalConverter(string input) =>
            decimal.TryParse(input, CultureInfo.InvariantCulture, out var output)
                ? new Tuple<bool, string, decimal>(true, "Correct parameter", output)
                : new Tuple<bool, string, decimal>(false, "Not valid decimal format", output);

        private static Tuple<bool, string, char> CharConverter(string input) =>
            string.IsNullOrWhiteSpace(input) || input.Length > 1
                ? new Tuple<bool, string, char>(false, "Gender can't have more than 1 symbol", ' ')
                : new Tuple<bool, string, char>(true, "Correct parameter", input.ToUpperInvariant()[0]);

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            while (true)
            {
                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                var value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
        }
    }
}