using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp;

public class UpdateCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "update")
{
    protected override void HandleCore(string parameters)
    {
        var match = Regex.Match(parameters, @"set\s+(.*?)\s*(where\s+(.*))", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            Console.WriteLine(
                "Invalid update command format. Use: update set <ColumnName> = '<Value>' where <ColumnName> = '<Value>'");
            return;
        }

        var setPart = match.Groups[1].Value;
        var wherePart = match.Groups[2].Value;

        var setClauses = ParseKeyValuePairs(setPart);
        var whereClauses = ParseKeyValuePairs(wherePart);

        if (setClauses.Count == 0)
        {
            Console.WriteLine("Set part must have fields and their values. Values must be in ' '");
            return;
        }

        if (whereClauses.Count == 0)
        {
            Console.WriteLine("Where part must have fields and their values. Values must be in ' '");
            return;
        }

        if (setClauses.ContainsKey("id"))
        {
            Console.WriteLine("Can't change id field.");
            return;
        }

        HashSet<string> allowedFilters = new (StringComparer.OrdinalIgnoreCase) { "id", "firstName", "lastName", "dateOfBirth" };

        var invalidKeys = whereClauses.Keys.Where(key => !allowedFilters.Contains(key)).ToList();
        if (invalidKeys.Count != 0)
        {
            Console.WriteLine($"Error: Invalid filter(s) detected: {string.Join(", ", invalidKeys)}. Allowed filters: {string.Join(", ", allowedFilters)}.");
            return;
        }

        List<FileCabinetRecord> foundRecords = [];

        if (whereClauses.TryGetValue("ID", out var id))
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
            if (whereClauses.TryGetValue("DATEOFBIRTH", out var dateOfBirth))
            {
                foundRecords = this.fileCabinetService.FindByDateOfBirth(dateOfBirth).ToList();
            }
            else
            {
                foundRecords = this.fileCabinetService.GetRecords().ToList();
            }

            foundRecords = foundRecords.Where(record =>
                (!whereClauses.ContainsKey("FIRSTNAME") || record.FirstName.Equals(whereClauses["FIRSTNAME"], StringComparison.OrdinalIgnoreCase)) &&
                (!whereClauses.ContainsKey("LASTNAME") || record.LastName.Equals(whereClauses["LASTNAME"], StringComparison.OrdinalIgnoreCase))
            ).ToList();
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
                var recordParams = CreateRecordsParametes(setClauses, record);
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

    private static FileCabinetRecordsParameters CreateRecordsParametes(Dictionary<string, string> setParams,
        FileCabinetRecord record)
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