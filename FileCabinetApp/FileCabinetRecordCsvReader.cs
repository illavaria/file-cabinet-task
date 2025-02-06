using System.Globalization;

namespace FileCabinetApp;

/// <summary>
/// Class represents csv reader for file cabinet records.
/// </summary>
/// <param name="reader">Stream reader used to read.</param>
public class FileCabinetRecordCsvReader(StreamReader reader)
{
    private StreamReader reader = reader ?? throw new ArgumentNullException(nameof(reader));

    /// <summary>
    /// Reads all records from reader.
    /// </summary>
    /// <returns>List of records.</returns>
    public IList<FileCabinetRecord> ReadAll()
    {
        var records = new List<FileCabinetRecord>();

        while (this.reader.Peek() >= 0)
        {
            var recordString = this.reader.ReadLine();
            var fields = recordString?.Split(',');
            if (fields?.Length != 7)
            {
                throw new FormatException("Invalid line format. Expected 7 fields.");
            }

            records.Add(new FileCabinetRecord
            {
                Id = int.Parse(fields[0], CultureInfo.InvariantCulture),
                FirstName = fields[1],
                LastName = fields[2],
                DateOfBirth = DateTime.ParseExact(fields[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                NumberOfChildren = short.Parse(fields[4], NumberFormatInfo.InvariantInfo),
                YearIncome = decimal.Parse(fields[5], CultureInfo.InvariantCulture),
                Gender = char.Parse(fields[6]),
            });
        }

        return records;
    }
}