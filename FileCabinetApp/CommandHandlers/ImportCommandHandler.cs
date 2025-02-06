using System.Collections.ObjectModel;
using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for import operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class ImportCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "import")
{
    private new readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));

    private static readonly (string, Action<FileCabinetServiceSnapshot, StreamReader>)[] importParams =
    [
        new ("csv", LoadFromCsv),
        new ("xml", LoadFromXml)
    ];

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        _ = string.IsNullOrWhiteSpace(parameters) ? throw new ArgumentException("This command takes parameters") : parameters;
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
            var validationErrors = new Collection<string>();
            this.fileCabinetService.Restore(snapshot, ref validationErrors);
            foreach (var error in validationErrors)
            {
                Console.WriteLine(error);
            }

            Console.WriteLine(
                $"{snapshot.Records.Count - validationErrors.Count} records were imported from file {filePath}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Import failed: {ex.Message}");
        }
    }

    private static void LoadFromCsv(FileCabinetServiceSnapshot snapshot, StreamReader reader) =>
        snapshot.LoadFromCsv(reader);

    private static void LoadFromXml(FileCabinetServiceSnapshot snapshot, StreamReader reader) =>
        snapshot.LoadFromXml(reader);
}