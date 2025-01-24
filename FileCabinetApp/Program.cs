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

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;

        private static string[] commands = ["help", "exit", "stat", "create", "list", "edit", "find", "export", "import", "remove", "purge"];

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
            var storageRule = "memory";
            if (args?.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("--validation-rules=", StringComparison.OrdinalIgnoreCase))
                    {
                        validationRule = args[i]["--validation-rules=".Length..];
                    }
                    else if (string.Equals(args[i], "-v", StringComparison.OrdinalIgnoreCase))
                    {
                        validationRule = args[i + 1];
                        i++;
                    }
                    else if (args[i].StartsWith("--storage=", StringComparison.OrdinalIgnoreCase))
                    {
                        storageRule = args[i]["--storage=".Length..];
                    }
                    else if (string.Equals(args[i], "-s", StringComparison.OrdinalIgnoreCase))
                    {
                        storageRule = args[i + 1];
                        i++;
                    }
                }
            }

            var index = Array.FindIndex(validationParams, i => i.Item1.Equals(validationRule, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                Console.WriteLine("Unknown validation rule");
                return;
            }

            switch (storageRule)
            {
                case "memory":
                    Program.fileCabinetService = new FileCabinetMemoryService(validationParams[index].Item2.Invoke());
                    break;
                case "file":
                    Program.fileCabinetService =
                        new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite),validationParams[index].Item2.Invoke());
                    break;
                default:
                {
                    Console.WriteLine("Unknown storage rule");
                    return;
                }
            }

            Program.validator = validationParams[index].Item2.Invoke();

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

                if (commands.FirstOrDefault(c => c.Equals(command, StringComparison.OrdinalIgnoreCase)) is not null)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                    var commandHandler = CreateCommandHandlers();
                    commandHandler.Handle(new AppCommandRequest { Command = command, Parameters = parameters });
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

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var exitHandler = new ExitCommandHandler(state => isRunning = state);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);

            helpHandler.SetNext(exitHandler);
            exitHandler.SetNext(statHandler);
            statHandler.SetNext(createHandler);
            createHandler.SetNext(listHandler);
            listHandler.SetNext(editHandler);
            editHandler.SetNext(findHandler);
            findHandler.SetNext(exportHandler);
            exportHandler.SetNext(importHandler);
            importHandler.SetNext(removeHandler);
            removeHandler.SetNext(purgeHandler);

            return helpHandler;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        public static void InputParameters(out string? firstName, out string? lastName, out DateTime dateOfBirth, out short numberOfChildren, out decimal yearIncome, out char gender)
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