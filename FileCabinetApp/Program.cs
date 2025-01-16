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

        private static Tuple<string, Action<string>>[] commands =
        [
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List)
        ];

        private static string[][] helpMessages =
        [
            ["help", "prints the help screen", "The 'help' command prints the help screen."],
            ["exit", "exits the application", "The 'exit' command exits the application."],
            ["stat", "prints the statistics of records", "The 'stat' command prints the statistics of records"],
            ["create", "creates a new record", "The 'create' command creates a new record"],
            ["list", "prints all records", "The 'list' command prints prints information about all records."]
        ];

        private static FileCabinetService fileCabinetService = new ();

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
            Console.Write("First name: ");
            var firstName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(firstName))
            {
                Console.WriteLine("First name can't be empty");
                return;
            }

            Console.Write("Last name: ");
            var lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Last name can't be empty");
                return;
            }

            Console.Write("Date of birth: ");
            var input = Console.ReadLine();
            if (!DateTime.TryParse(input, out var dateOfBirth))
            {
                Console.WriteLine("Wrong argument for date of birth");
                return;
                //add new try
                //maybe check names too
                //можно сделать цикл, чтобы давать возможность ввести правильные символы или прекратить по нажатию кнопки
            }

            Console.Write("Number of children: ");
            input = Console.ReadLine();
            if (!short.TryParse(input, out var numberOfChildren))
            {
                Console.WriteLine("Wrong argument for number of children");
                return;
            }

            Console.Write("Year income: ");
            input = Console.ReadLine();
            if (!decimal.TryParse(input, out var yearIncome))
            {
                Console.WriteLine("Wrong argument for year income");
                return;
            }

            Console.Write("Gender (M, F, N): ");
            input = Console.ReadLine();
            input = input?.ToUpper(CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(input) || input.Length > 1 || !(input[0] == 'M' || input[0] == 'F' || input[0] == 'N'))
            {
                Console.WriteLine("Wrong argument for gender");
                return;
            }

            var gender = input[0];

            var recordId = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, numberOfChildren, yearIncome, gender);
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }
}