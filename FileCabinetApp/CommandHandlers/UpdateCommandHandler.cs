using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for update operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class UpdateCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "update")
{
    private new readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        _ = string.IsNullOrWhiteSpace(parameters) ? throw new ArgumentException("This command takes parameters") : parameters;
        var match = Regex.Match(parameters, @"set\s+(.*?)\s*(where\s+(.*))", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            Console.WriteLine(
                "Invalid update command format. Use: update set <field> = '<Value>' where <field> = '<Value>'");
            return;
        }

        var setPart = match.Groups[1].Value;
        var wherePart = match.Groups[2].Value;

        var setClauses = ParseKeyValuePairs(setPart);
        var conditions = ParseKeyValuePairs(wherePart);

        if (setClauses.Count == 0)
        {
            Console.WriteLine("Set part must have fields and their values. Values must be in ' '");
            return;
        }

        if (conditions.Count == 0)
        {
            Console.WriteLine("Where part must have fields and their values. Values must be in ' '");
            return;
        }

        if (setClauses.ContainsKey("id"))
        {
            Console.WriteLine("Can't change id field.");
            return;
        }

        List<FileCabinetRecord> foundRecords = [];

        if (conditions.TryGetValue("ID", out var id))
        {
            var recordId = int.Parse(id, CultureInfo.InvariantCulture);
            var foundRecord = this.fileCabinetService.FindById(recordId);
            if (foundRecord is null)
            {
                Console.WriteLine($"Record #{recordId} is not found.");
                return;
            }

            foundRecords.Add(foundRecord);
        }
        else
        {
            try
            {
                foundRecords = this.fileCabinetService.Find(conditions).ToList();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        if (foundRecords.Count == 0)
        {
            Console.WriteLine("No records that satisfy the condition.");
            return;
        }

        foreach (var record in foundRecords)
        {
            try
            {
                var recordParams = CreateRecordsParameters(setClauses, record);
                this.fileCabinetService.EditRecord(record.Id, recordParams);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Couldn't update record {record.Id}: {e.Message}");
                continue;
            }

            Console.WriteLine($"Updated record {record.Id}");
        }
    }

    private static Dictionary<string, string> ParseKeyValuePairs(string input)
    {
        var result = new Dictionary<string, string>();
        var matches = Regex.Matches(input, @"(\w+)\s*=\s*'([^']*)'", RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                var key = match.Groups[1].Value.ToUpperInvariant();
                var value = match.Groups[2].Value;
                result[key] = value;
            }
        }

        return result;
    }

    private static FileCabinetRecordsParameters CreateRecordsParameters(Dictionary<string, string> setParams, FileCabinetRecord record)
    {
        var recordParams = new FileCabinetRecordsParameters(record);
        foreach (var (fieldName, value) in setParams)
        {
            switch (fieldName)
            {
                case "FIRSTNAME":
                    recordParams.FirstName = value;
                    break;
                case "LASTNAME":
                    recordParams.LastName = value;
                    break;
                case "DATEOFBIRTH":
                    recordParams.DateOfBirth = DateTime.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "NUMBEROFCHILDREN":
                    recordParams.NumberOfChildren = short.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "YEARINCOME":
                    recordParams.YearIncome = decimal.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "GENDER":
                    recordParams.Gender = char.Parse(value);
                    break;
                default:
                    throw new ArgumentException($"No field with the name {fieldName}");
            }
        }

        return recordParams;
    }
}