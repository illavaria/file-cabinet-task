using System.Collections.ObjectModel;

namespace FileCabinetApp;

public class FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator) : IFileCabinetService
{
    private const int RecordSize = 278;

    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        validator.ValidateParameters(parameters);

        var recordOffset = fileStream.Length;
        var recordId = ((int)recordOffset / RecordSize) + 1;
        fileStream.Seek(recordOffset, SeekOrigin.Begin);
        using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
        WriteRecord(recordId, parameters!, writer);

        writer.Flush();
        return recordId;
    }

    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        return this.ReadAllRecords((record, s) => true, string.Empty);
    }

    public int GetStat() => (int)(fileStream.Length / RecordSize) + 1;

    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        validator.ValidateParameters(parameters);
        var recordOffset = RecordSize * (id - 1);
        fileStream.Seek(recordOffset, SeekOrigin.Begin);
        using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        WriteRecord(id, parameters!, writer);

        writer.Flush();
    }


    public FileCabinetRecord? FindById(int id)
    {
        var recordOffset = RecordSize * (id - 1);
        if (recordOffset > fileStream.Length)
        {
            return null;
        }

        fileStream.Seek(recordOffset + 2, SeekOrigin.Begin);
        using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        return ReadRecord(reader);
    }

    public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string? firstName) =>
        this.ReadAllRecords((record, s) => record.FirstName == s, firstName);

    public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string? lastName) =>
        this.ReadAllRecords((record, s) => record.LastName == s, lastName);

    public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString) =>
        !DateTime.TryParse(dateOfBirthString, out var dateOfBirth)
            ? new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>())
            : this.ReadAllRecords((record, date) => record.DateOfBirth == date, dateOfBirth);

    public FileCabinetServiceSnapshot MakeSnapshot()
    {
        throw new NotImplementedException();
    }

    private static char[] PadString(string? value, int length)
    {
        var padded = new char[length];
        var valueChars = (value ?? string.Empty).ToCharArray();
        Array.Copy(valueChars, padded, Math.Min(valueChars.Length, length));
        return padded;
    }

    private static FileCabinetRecord ReadRecord(BinaryReader reader)
    {
        var id = reader.ReadInt32();
        var firstName = new string(reader.ReadChars(60)).TrimEnd('\0');
        var lastName = new string(reader.ReadChars(60)).TrimEnd('\0');
        var year = reader.ReadInt32();
        var month = reader.ReadInt32();
        var day = reader.ReadInt32();
        var numberOfChildren = reader.ReadInt16();
        var yearIncome = reader.ReadDecimal();
        var gender = reader.ReadChar();

        var record = new FileCabinetRecord
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = new DateTime(year, month, day),
            NumberOfChildren = numberOfChildren,
            YearIncome = yearIncome,
            Gender = gender,
        };
        return record;
    }

    private static void WriteRecord(int id, FileCabinetRecordsParameters? parameters, BinaryWriter writer)
    {
        writer.Write((short)1);
        writer.Write(id);
        writer.Write(PadString(parameters.FirstName, 60));
        writer.Write(PadString(parameters.LastName, 60));
        writer.Write(parameters.DateOfBirth.Year);
        writer.Write(parameters.DateOfBirth.Month);
        writer.Write(parameters.DateOfBirth.Day);
        writer.Write(parameters.NumberOfChildren);
        writer.Write(parameters.YearIncome);
        writer.Write(parameters.Gender);
    }

    private ReadOnlyCollection<FileCabinetRecord> ReadAllRecords<T>(Func<FileCabinetRecord, T, bool> condition, T checkString)
    {
        var records = new List<FileCabinetRecord>();
        fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (fileStream.Position < fileStream.Length)
        {
            var status = reader.ReadInt16();
            if (status != 1)
            {
                fileStream.Seek(RecordSize - 2, SeekOrigin.Current);
                continue;
            }

            var record = ReadRecord(reader);
            if (condition(record, checkString))
            {
                records.Add(record);
            }
        }

        return records.AsReadOnly();
    }
}