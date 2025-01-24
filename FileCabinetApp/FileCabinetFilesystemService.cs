using System.Collections.ObjectModel;

namespace FileCabinetApp;

/// <summary>
/// Class represents filesystem file cabinet service.
/// </summary>
public class FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator) : IFileCabinetService
{
    private const int RecordSize = 278;

    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        validator.ValidateParameters(parameters);
        var recordId = GetNextRecordId();
        fileStream.Seek(0, SeekOrigin.End);
        using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
        WriteRecord(recordId, parameters!, writer);

        writer.Flush();
        return recordId;

        int GetNextRecordId()
        {
            if (fileStream.Length == 0)
            {
                return 1;
            }

            fileStream.Seek(-RecordSize, SeekOrigin.End);
            var maxId = 0;

            while (fileStream.Position > 0)
            {
                using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

                var status = reader.ReadInt16();
                var id = reader.ReadInt32();

                if ((status & 0b0100) == 0)
                {
                    maxId = Math.Max(id, maxId);
                }

                fileStream.Seek(-RecordSize - 6, SeekOrigin.Current);
            }

            return maxId + 1;
        }
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        return this.ReadAllRecords((record, s) => true, string.Empty);
    }

    /// <inheritdoc/>
    public int GetNumberOfAllRecords() => (int)(fileStream.Length / RecordSize);

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords()
    {
        var count = 0;
        fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (fileStream.Position < fileStream.Length)
        {
            var status = reader.ReadInt16();
            if ((status & 0b0100) != 0)
            {
                count++;
            }

            fileStream.Seek(RecordSize - 2, SeekOrigin.Current);
        }

        return count;
    }

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        validator.ValidateParameters(parameters);

        fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (fileStream.Position < fileStream.Length)
        {
            var status = reader.ReadInt16();
            if ((status & 0b0100) != 0)
            {
                fileStream.Seek(RecordSize - 2, SeekOrigin.Current);
                continue;
            }

            var record = ReadRecord(reader);
            if (record.Id == id)
            {
                fileStream.Seek(-RecordSize, SeekOrigin.Current);
                break;
            }
        }

        if (fileStream.Position >= fileStream.Length)
        {
            throw new ArgumentException($"#{id} record is not found.");
        }

        using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
        WriteRecord(id, parameters!, writer);
        writer.Flush();
    }

    /// <inheritdoc/>
    public void RemoveRecord(int id)
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (fileStream.Position < fileStream.Length)
        {
            var status = reader.ReadInt16();
            if ((status & 0b0100) != 0)
            {
                fileStream.Seek(RecordSize - 2, SeekOrigin.Current);
                continue;
            }

            var record = ReadRecord(reader);
            if (record.Id == id)
            {
                fileStream.Seek(-RecordSize, SeekOrigin.Current);
                using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
                writer.Write((short)(status | 0b0100));
                writer.Flush();
                break;
            }
        }

        if (fileStream.Position >= fileStream.Length)
        {
            throw new ArgumentException($"#{id} record is not found.");
        }
    }

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id) =>
        this.ReadAllRecords((record, i) => record.Id == i, id).SingleOrDefault();
    // {
    //     var recordOffset = RecordSize * (id - 1);
    //     if (recordOffset > fileStream.Length)
    //     {
    //         return null;
    //     }
    //
    //     fileStream.Seek(recordOffset + 2, SeekOrigin.Begin);
    //     using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
    //
    //     return ReadRecord(reader);
    // }

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string? firstName) =>
        this.ReadAllRecords((record, s) => record.FirstName == s, firstName);

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string? lastName) =>
        this.ReadAllRecords((record, s) => record.LastName == s, lastName);

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString) =>
        !DateTime.TryParse(dateOfBirthString, out var dateOfBirth)
            ? new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>())
            : this.ReadAllRecords((record, date) => record.DateOfBirth == date, dateOfBirth);

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot() => new FileCabinetServiceSnapshot(this.GetRecords().ToArray());

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref List<string> errorsList)
    {
        _ = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        foreach (var record in snapshot.Records)
        {
            try
            {
                var parameters = new FileCabinetRecordsParameters
                {
                    FirstName = record.FirstName, LastName = record.LastName, DateOfBirth = record.DateOfBirth,
                    NumberOfChildren = record.NumberOfChildren, YearIncome = record.YearIncome, Gender = record.Gender,
                };
                validator.ValidateParameters(parameters);

                fileStream.Seek(0, SeekOrigin.Begin);
                using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

                while (fileStream.Position < fileStream.Length)
                {
                    var status = reader.ReadInt16();
                    if ((status & 0b0100) != 0)
                    {
                        fileStream.Seek(RecordSize - 2, SeekOrigin.Current);
                        continue;
                    }

                    var recordRead = ReadRecord(reader);
                    if (recordRead.Id == record.Id)
                    {
                        fileStream.Seek(-RecordSize, SeekOrigin.Current);
                        break;
                    }
                }

                using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
                WriteRecord(record.Id, parameters, writer);
                writer.Flush();
            }
            catch (Exception e)
            {
                errorsList?.Add($"Record #{record.Id} didn't pass validation: {e.Message}");
            }
        }
    }

    /// <inheritdoc/>
    public void Purge()
    {
        fileStream.Seek(0, SeekOrigin.Begin);

        long writePosition = 0;
        long readPosition = 0;

        while (readPosition < fileStream.Length)
        {
            fileStream.Seek(readPosition, SeekOrigin.Begin);
            using var reader = new BinaryReader(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
            using var writer = new BinaryWriter(fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

            var status = reader.ReadInt16();
            var recordBytes = reader.ReadBytes(RecordSize - 2);

            if ((status & 0b0100) == 0)
            {
                if (writePosition != readPosition)
                {
                    fileStream.Seek(writePosition, SeekOrigin.Begin);
                    writer.Write(status);
                    writer.Write(recordBytes);
                }

                writePosition += RecordSize;
            }

            readPosition += RecordSize;
        }

        fileStream.SetLength(writePosition);
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
        writer.Write((short)0);
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
            if ((status & 0b0100) != 0)
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