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
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
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
            ["find", "finds records", "The 'find' command prints records with the needed value."],
            ["export", "exports records", "The 'export' command exports records to a file."],
            ["import", "imports records", "The 'import' command imports records from file."],
            ["remove", "removes the record", "The 'remove' command removes the record by its id"],
            ["purge", "removes deleted records", "The 'purge' command removes deleted records"]
        ];

        private static Tuple<string, Func<IRecordValidator>>[] validationParams =
        [
            new Tuple<string, Func<IRecordValidator>>("custom", () => new CustomValidator()),
            new Tuple<string, Func<IRecordValidator>>("default", () => new DefaultValidator())
        ];

        private static Tuple<string, Action<FileCabinetServiceSnapshot, StreamWriter>>[] exportParams =
        [
            new Tuple<string, Action<FileCabinetServiceSnapshot, StreamWriter>>("csv", SaveToCsv),
            new Tuple<string, Action<FileCabinetServiceSnapshot, StreamWriter>>("xml", SaveToXml)
        ];

        private static Tuple<string, Action<FileCabinetServiceSnapshot, StreamReader>>[] importParams =
        [
            new Tuple<string, Action<FileCabinetServiceSnapshot, StreamReader>>("csv", LoadFromCsv),
            new Tuple<string, Action<FileCabinetServiceSnapshot, StreamReader>>("xml", LoadFromXml)
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
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("This command doesn't take any arguments");
                return;
            }

            Console.WriteLine($"Number of all records: {fileCabinetService.GetNumberOfAllRecords()}.");
            Console.WriteLine($"Number of deleted records: {fileCabinetService.GetNumberOfDeletedRecords()}.");
        }

        private static void Create(string parameters)
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
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("This command doesn't take any arguments");
                return;
            }

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

        private static void Export(string parameters)
        {
            var args = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length < 2)
            {
                Console.WriteLine("Command must have 2 parameters: export type and filepath");
                return;
            }

            var index = Array.FindIndex(exportParams, i => i.Item1.Equals(args[0], StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                Console.WriteLine("Wrong export type.");
                return;
            }

            var filePath = args[1];
            var fileExtension = Path.GetExtension(filePath).Trim('.');
            if (!string.Equals(fileExtension, exportParams[index].Item1, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("File extension must be the same as chosen export type");
                return;
            }

            if (File.Exists(filePath))
            {
                Console.Write($"File '{filePath}' already exists. Overwrite? [Y/n] ");
                var input = Console.ReadLine()?.Trim();
                if (!string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Export canceled.");
                    return;
                }
            }

            var saveCommand = exportParams[index].Item2;
            try
            {
                using var streamWriter = new StreamWriter(filePath);
                var snapshot = Program.fileCabinetService.MakeSnapshot();
                saveCommand(snapshot, streamWriter);
                Console.WriteLine($"All records are exported to file {filePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export failed: {ex.Message}");
            }
        }

        private static void Import(string parameters)
        {
            var args = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length < 2)
            {
                Console.WriteLine("Command must have 2 parameters: import type and filepath");
                return;
            }

            var index = Array.FindIndex(importParams, i => i.Item1.Equals(args[0], StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                Console.WriteLine("Wrong import type.");
                return;
            }

            var filePath = args[1];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Import error: file {filePath} is not exist.");
                return;
            }

            var fileExtension = Path.GetExtension(filePath).Trim('.');
            if (!string.Equals(fileExtension, importParams[index].Item1, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("File extension must be the same as chosen export type");
                return;
            }

            var loadCommand = importParams[index].Item2;
            try
            {
                using var streamReader = new StreamReader(filePath);
                var snapshot = new FileCabinetServiceSnapshot();
                loadCommand(snapshot, streamReader);
                var validationErrors = new List<string>();
                fileCabinetService.Restore(snapshot, ref validationErrors);
                foreach (var error in validationErrors)
                {
                    Console.WriteLine(error);
                }

                Console.WriteLine($"{snapshot.Records.Count - validationErrors.Count} records were imported from file {filePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import failed: {ex.Message}");
            }
        }

        private static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, CultureInfo.InvariantCulture, out var recordId))
            {
                Console.WriteLine("Pass one number as a record id");
                return;
            }

            try
            {
                Program.fileCabinetService.RemoveRecord(recordId);
                Console.WriteLine($"Record {recordId} is removed.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Purge(string parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("This command doesn't take any arguments");
                return;
            }

            var recordBefore = fileCabinetService.GetNumberOfAllRecords();
            fileCabinetService.Purge();
            Console.WriteLine(
                $"Data file processing is completed: {recordBefore - fileCabinetService.GetNumberOfAllRecords()} of {recordBefore} records were purged.");
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

        private static void SaveToCsv(FileCabinetServiceSnapshot snapshot, StreamWriter writer) =>
            snapshot.SaveToCsv(writer);

        private static void SaveToXml(FileCabinetServiceSnapshot snapshot, StreamWriter writer) =>
            snapshot.SaveToXml(writer);


        private static void LoadFromCsv(FileCabinetServiceSnapshot snapshot, StreamReader reader) =>
            snapshot.LoadFromCsv(reader);

        private static void LoadFromXml(FileCabinetServiceSnapshot snapshot, StreamReader reader) =>
            snapshot.LoadFromXml(reader);


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