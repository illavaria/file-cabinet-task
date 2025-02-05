using System.Globalization;
using System.Reflection;
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

    private static bool CheckRecordSatisfiesConditions(FileCabinetRecord record, Dictionary<string, string> conditions)
    {
        foreach (var (field, value) in conditions)
        {
            var property = typeof(FileCabinetRecord).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Records don't have {field} field");
            }

            var recordValue = property.GetValue(record);
            var convertedValue = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);

            if (!object.Equals(recordValue, convertedValue))
            {
                return false;
            }
        }

        return true;
    }
}