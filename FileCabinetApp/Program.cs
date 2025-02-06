using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.FileCabinetServices;
using FileCabinetApp.InputValidators;
using FileCabinetApp.Validators;
using Newtonsoft.Json;

namespace FileCabinetApp
{
    /// <summary>
    /// Console client program.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Illaria Samal";
        private const string HintMessage = "Enter your command, or enter 'help' to see the available commands and their parameters.";
        private const string LogFilePath = "LogFile.txt";
        private const string FileSystemFilePath = "cabinet-records.db";
        private const string ValidationRuleFilePath = "validation-rules.json";

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService;
        private static IRecordConsoleInputValidator inputValidator;

        /// <summary>
        /// Main program body.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");

            var validationRule = "default";
            var storageRule = "memory";
            var useStopWatch = false;
            var useLogger = false;
            TextWriter? stopWatchWriter = null;

            if (args?.Length > 0)
            {
                for (var i = 0; i < args.Length; i++)
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
                    else if (args[i].StartsWith("-use-stopwatch", StringComparison.OrdinalIgnoreCase))
                    {
                        useStopWatch = true;
                        if (i + 1 < args.Length && args[i + 1].StartsWith("-console", StringComparison.OrdinalIgnoreCase))
                        {
                            stopWatchWriter = Console.Out;
                        }

                        i++;
                    }
                    else if (args[i].StartsWith("-use-logger", StringComparison.OrdinalIgnoreCase))
                    {
                        useLogger = true;
                    }
                }
            }

            if (!File.Exists(ValidationRuleFilePath))
            {
                Console.WriteLine($"Validation file {ValidationRuleFilePath} not found.");
                return;
            }

            var json = File.ReadAllText(ValidationRuleFilePath);
            var validationSettings = JsonConvert.DeserializeObject<ValidationSettings.ValidationSettings>(json);
            if (validationSettings is null)
            {
                Console.WriteLine("Validation file must have correct parameters");
                return;
            }

            IRecordValidator validator;

            try
            {
                if (string.Equals(validationRule, "default", StringComparison.OrdinalIgnoreCase))
                {
                    if (validationSettings.Default is null)
                    {
                        Console.WriteLine("Check validation file. It must have default parameters.");
                        return;
                    }

                    Program.inputValidator = new InputValidator(validationSettings.Default);
                    validator = new ValidatorBuilder().CreateDefault(validationSettings.Default);
                    Console.WriteLine("Program is running with default validation setting.");
                }
                else if (string.Equals(validationRule, "custom", StringComparison.OrdinalIgnoreCase))
                {
                    if (validationSettings.Custom is null)
                    {
                        Console.WriteLine("Check validation file. It must have default parameters.");
                        return;
                    }

                    Program.inputValidator = new InputValidator(validationSettings.Custom);
                    validator = new ValidatorBuilder().CreateCustom(validationSettings.Custom);
                    Console.WriteLine("Program is running with custom validation setting.");
                }
                else
                {
                    Console.WriteLine("Unknown validation rule");
                    return;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"Couldn't get validation settings: {e.Message}");
                return;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Couldn't get validation settings: {e.Message}");
                return;
            }

            switch (storageRule)
            {
                case "memory":
                    fileCabinetService = new FileCabinetMemoryService(validator);
                    Console.WriteLine("Program is running with as memory service.");
                    break;
                case "file":
                    fileCabinetService =
                        new FileCabinetFileSystemService(new FileStream(FileSystemFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite), validator);
                    Console.WriteLine("Program is running with as file system service.");
                    break;
                default:
                {
                    Console.WriteLine("Unknown storage rule");
                    return;
                }
            }

            using var streamWriter = new StreamWriter(LogFilePath, true);
            stopWatchWriter ??= streamWriter;

            if (useStopWatch)
            {
                fileCabinetService = new FileCabinetServiceMeter(fileCabinetService, stopWatchWriter);
                Console.WriteLine($"Execution time is written to {(stopWatchWriter == Console.Out ? "console" : $"logfile {LogFilePath}")}");
            }

            if (useLogger)
            {
                streamWriter.AutoFlush = true;
                fileCabinetService = new FileCabinetServiceLogger(fileCabinetService, streamWriter);
                Console.WriteLine($"Log is written to {LogFilePath}");
            }

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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                var commandHandler = CreateCommandHandlers(command);
                commandHandler.Handle(new AppCommandRequest { Command = command, Parameters = parameters });
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers(string command)
        {
            var helpHandler = new HelpCommandHandler();
            var exitHandler = new ExitCommandHandler(state => isRunning = state);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var createHandler = new CreateCommandHandler(fileCabinetService, InputParameters);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var editHandler = new EditCommandHandler(fileCabinetService, InputParameters);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);
            var selectHandler = new SelectCommandHandler(fileCabinetService);
            var unknownHandler = new UnknownCommandHandler(command);

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
            purgeHandler.SetNext(insertHandler);
            insertHandler.SetNext(deleteHandler);
            deleteHandler.SetNext(updateHandler);
            updateHandler.SetNext(selectHandler);
            selectHandler.SetNext(unknownHandler);

            return helpHandler;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        private static FileCabinetRecordsParameters InputParameters()
        {
            Console.Write("First name: ");
            var firstName = ReadInput(StringConverter, inputValidator.FirstNameValidator);
            Console.Write("Last name: ");
            var lastName = ReadInput(StringConverter, inputValidator.LastNameValidator);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(DateConverter, inputValidator.DateOfBirthValidator);

            Console.Write("Number of children: ");
            var numberOfChildren = ReadInput(ShortConverter, inputValidator.NumberOfChildrenValidator);

            Console.Write("Year income: ");
            var yearIncome = ReadInput(DecimalConverter, inputValidator.YearIncomeValidator);

            Console.Write("Gender (M, F, N): ");
            var gender = ReadInput(CharConverter, inputValidator.GenderValidator);

            return new FileCabinetRecordsParameters
            {
                FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth,
                NumberOfChildren = numberOfChildren, YearIncome = yearIncome, Gender = gender,
            };
        }

        private static Tuple<bool, string, string> StringConverter(string? input) =>
            string.IsNullOrWhiteSpace(input)
                ? new Tuple<bool, string, string>(false, "Input string can't be empty", string.Empty)
                : new (true, "Correct parameter", input);

        private static Tuple<bool, string, DateTime> DateConverter(string? input) =>
            DateTime.TryParse(input, CultureInfo.InvariantCulture, out var output)
                ? new Tuple<bool, string, DateTime>(true, "Correct parameter", output)
                : new Tuple<bool, string, DateTime>(false, "Not valid date format", output);

        private static Tuple<bool, string, short> ShortConverter(string? input) =>
            short.TryParse(input, CultureInfo.InvariantCulture, out var output)
                ? new Tuple<bool, string, short>(true, "Correct parameter", output)
                : new Tuple<bool, string, short>(false, "Not valid number format", output);

        private static Tuple<bool, string, decimal> DecimalConverter(string? input) =>
            decimal.TryParse(input, CultureInfo.InvariantCulture, out var output)
                ? new Tuple<bool, string, decimal>(true, "Correct parameter", output)
                : new Tuple<bool, string, decimal>(false, "Not valid decimal format", output);

        private static Tuple<bool, string, char> CharConverter(string? input) =>
            string.IsNullOrWhiteSpace(input) || input.Length > 1
                ? new Tuple<bool, string, char>(false, "Gender can't have more than 1 symbol", ' ')
                : new Tuple<bool, string, char>(true, "Correct parameter", input.ToUpperInvariant()[0]);

        private static T ReadInput<T>(Func<string?, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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