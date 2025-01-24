namespace FileCabinetApp;

public class ExportCommandHandler(IFileCabinetService fileCabinetService) : ServiceCommandHandleBase(fileCabinetService)
{
    private const string CommandName = "export";
    private static Tuple<string, Action<FileCabinetServiceSnapshot, StreamWriter>>[] exportParams =
    [
        new Tuple<string, Action<FileCabinetServiceSnapshot, StreamWriter>>("csv", SaveToCsv),
        new Tuple<string, Action<FileCabinetServiceSnapshot, StreamWriter>>("xml", SaveToXml)
    ];

    /// <inheritdoc/>
    public override void Handle(AppCommandRequest commandRequest)
    {
        _ = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));

        if (!commandRequest.Command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
        {
            this.NextHandler.Handle(commandRequest);
            return;
        }

        this.Export(commandRequest.Parameters);
    }

    private static void SaveToCsv(FileCabinetServiceSnapshot snapshot, StreamWriter writer) =>
        snapshot.SaveToCsv(writer);

    private static void SaveToXml(FileCabinetServiceSnapshot snapshot, StreamWriter writer) =>
        snapshot.SaveToXml(writer);

    private void Export(string parameters)
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
            var snapshot = this.fileCabinetService.MakeSnapshot();
            saveCommand(snapshot, streamWriter);
            Console.WriteLine($"All records are exported to file {filePath}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Export failed: {ex.Message}");
        }
    }
}