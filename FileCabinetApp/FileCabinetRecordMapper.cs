using System.Globalization;

namespace FileCabinetApp;

/// <summary>
/// Class represents records mapper.
/// </summary>
public static class FileCabinetRecordMapper
{
    /// <summary>
    /// Maps record to xml record.
    /// </summary>
    /// <param name="record">FileCabinetRecord record to map.</param>
    /// <returns>Record mapped to FileCabinetRecordXml.</returns>
    /// <exception cref="ArgumentNullException">Thrown if record is null.</exception>
    public static FileCabinetRecordXml MapToXml(FileCabinetRecord record)
    {
        _ = record ?? throw new ArgumentNullException(nameof(record));
        var recordXml = new FileCabinetRecordXml()
        {
            Id = record.Id,
            Name = new Name() { FirstName = record.FirstName, LastName = record.LastName },
            DateOfBirth = record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            NumberOfChildren = record.NumberOfChildren,
            YearIncome = record.YearIncome,
            Gender = record.Gender.ToString(),
        };
        return recordXml;
    }

    /// <summary>
    /// Maps xml record to record.
    /// </summary>
    /// <param name="xmlRecord">FileCabinetRecordXml record to map.</param>
    /// <returns>Record mapped to FileCabinetRecord.</returns>
    /// <exception cref="ArgumentNullException">Thrown if record is null.</exception>
    public static FileCabinetRecord MapFromXml(FileCabinetRecordXml xmlRecord)
    {
        _ = xmlRecord ?? throw new ArgumentNullException(nameof(xmlRecord));

        var record = new FileCabinetRecord()
        {
            Id = xmlRecord.Id,
            FirstName = xmlRecord.Name!.FirstName,
            LastName = xmlRecord.Name.LastName,
            DateOfBirth = DateTime.ParseExact(xmlRecord.DateOfBirth!, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            NumberOfChildren = xmlRecord.NumberOfChildren,
            YearIncome = xmlRecord.YearIncome,
            Gender = char.Parse(xmlRecord.Gender),
        };

        return record;
    }
}