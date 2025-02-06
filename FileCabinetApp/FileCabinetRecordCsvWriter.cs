namespace FileCabinetApp;

/// <summary>
/// Class for writing records to csv file.
/// </summary>
public class FileCabinetRecordCsvWriter(TextWriter writer)
{
    private TextWriter writer = writer ?? throw new ArgumentNullException(nameof(writer));

    /// <summary>
    /// Writes a record in csv format.
    /// </summary>
    /// <param name="record">A record to write.</param>
    /// <exception cref="ArgumentNullException">Thrown if record is null.</exception>
    public void Write(FileCabinetRecord record)
    {
        _ = record ?? throw new ArgumentNullException(nameof(record));
        this.writer.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth:yyyy-MM-dd},{record.NumberOfChildren},{record.YearIncome},{record.Gender}");
    }
}