using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.FileCabinetServices;

namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents command handler for insert operation.
/// </summary>
/// <param name="fileCabinetService">File cabinet service command is operated in.</param>
public class InsertCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "insert")
{
    private new readonly IFileCabinetService fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));

    /// <inheritdoc/>
    protected override void HandleCore(string? parameters)
    {
        _ = string.IsNullOrWhiteSpace(parameters) ? throw new ArgumentException("This command takes parameters") : parameters;
        var match = Regex.Match(parameters, @"\((.*?)\)\s+values\s+\((.*?)\)", RegexOptions.IgnoreCase);
        var fieldNames = match.Groups[1].Value.Split(',', StringSplitOptions.TrimEntries);
        var values = match.Groups[2].Value.Split(',', StringSplitOptions.TrimEntries);

        if (fieldNames.Length != values.Length)
        {
            Console.WriteLine("Mismatch between field names and values.");
            return;
        }

        if (values.Length != 7 || fieldNames.Length != 7)
        {
            Console.WriteLine("The number of fields and values must be 7.");
            return;
        }

        var record = new FileCabinetRecordsParameters();
        var id = 0;
        for (var i = 0; i < fieldNames.Length; i++)
        {
            var field = fieldNames[i].ToLowerInvariant();
            var value = values[i].Trim('\'');

            try
            {
                switch (field)
                {
                    case "id":
                        id = int.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "firstname":
                        record.FirstName = value;
                        break;
                    case "lastname":
                        record.LastName = value;
                        break;
                    case "dateofbirth":
                        record.DateOfBirth = DateTime.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "numberofchildren":
                        record.NumberOfChildren = short.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "yearincome":
                        record.YearIncome = decimal.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "gender":
                        record.Gender = value[0];
                        break;
                    default:
                        Console.WriteLine($"Unknown field: {field}");
                        return;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error parsing field '{field}' with value '{value}': {ex.Message}");
                return;
            }
        }

        try
        {
            this.fileCabinetService.InsertRecord(id, record);
            Console.WriteLine($"Record #{id} is inserted.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error inserting the record: {ex.Message}");
        }
    }
}
