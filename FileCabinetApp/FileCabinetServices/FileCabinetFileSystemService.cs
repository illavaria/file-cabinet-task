using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileCabinetServices;

/// <summary>
/// Class represents filesystem file cabinet service.
/// </summary>
public class FileCabinetFileSystemService : IFileCabinetService
{
    private const int RecordSize = 278;
    private readonly FileStream fileStream;
    private readonly IRecordValidator recordValidator;
    private readonly Dictionary<string, List<long>> firstNameDictionary;
    private readonly Dictionary<string, List<long>> lastNameDictionary;
    private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetFileSystemService"/> class.
    /// </summary>
    /// <param name="fileStream">File stream used to store records.</param>
    /// <param name="recordValidator">Record's validator.</param>
    public FileCabinetFileSystemService(FileStream fileStream, IRecordValidator recordValidator)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(recordValidator);
        this.fileStream = fileStream;
        this.recordValidator = recordValidator;
        this.firstNameDictionary = new Dictionary<string, List<long>>();
        this.lastNameDictionary = new Dictionary<string, List<long>>();
        this.dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

        this.fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
        while (this.fileStream.Position < this.fileStream.Length)
        {
            var recordOffset = this.fileStream.Position;
            var (status, record) = ReadRecord(reader);
            if ((status & 0b0100) == 0)
            {
                AddToDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                AddToDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                AddToDictionary(this.dateOfBirthDictionary, record.DateOfBirth, recordOffset);
            }
        }
    }

    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        var recordId = GetNextRecordId();
        this.InsertRecord(recordId, parameters);
        return recordId;

        int GetNextRecordId()
        {
            if (this.fileStream.Length == 0)
            {
                return 1;
            }

            this.fileStream.Seek(-RecordSize, SeekOrigin.End);
            using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
            var maxId = 1;

            while (this.fileStream.Position > 0)
            {
                var status = reader.ReadInt16();
                var id = reader.ReadInt32();

                if ((status & 0b0100) == 0)
                {
                    maxId = Math.Max(id, maxId);
                }

                this.fileStream.Seek(-RecordSize - 6, SeekOrigin.Current);
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
    public int GetNumberOfAllRecords() => (int)(this.fileStream.Length / RecordSize);

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords()
    {
        var count = 0;
        this.fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (this.fileStream.Position < this.fileStream.Length)
        {
            var status = reader.ReadInt16();
            if ((status & 0b0100) != 0)
            {
                count++;
            }

            this.fileStream.Seek(RecordSize - 2, SeekOrigin.Current);
        }

        return count;
    }

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        this.recordValidator.ValidateParameters(parameters);

        this.fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (this.fileStream.Position < this.fileStream.Length)
        {
            var (status, record) = ReadRecord(reader);
            if ((status & 0b0100) == 0 && record.Id == id)
            {
                var recordOffset = this.fileStream.Position;
                if (!string.Equals(record.FirstName, parameters!.FirstName, StringComparison.OrdinalIgnoreCase))
                {
                    RemoveFromDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                    AddToDictionary(this.firstNameDictionary, parameters.FirstName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                }

                if (!string.Equals(record.LastName, parameters.LastName, StringComparison.OrdinalIgnoreCase))
                {
                    RemoveFromDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                    AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                }

                if (record.DateOfBirth != parameters.DateOfBirth)
                {
                    RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth, recordOffset);
                    AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, recordOffset);
                    record.DateOfBirth = parameters.DateOfBirth;
                }

                this.fileStream.Seek(-RecordSize, SeekOrigin.Current);
                break;
            }
        }

        if (this.fileStream.Position >= this.fileStream.Length)
        {
            throw new ArgumentException($"#{id} record is not found.");
        }

        using var writer = new BinaryWriter(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
        WriteRecord(id, parameters, writer);
        writer.Flush();
    }

    /// <inheritdoc/>
    public void RemoveRecord(int id)
    {
        this.fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (this.fileStream.Position < this.fileStream.Length)
        {
            var (status, record) = ReadRecord(reader);
            if ((status & 0b0100) == 0 && record.Id == id)
            {
                this.fileStream.Seek(-RecordSize, SeekOrigin.Current);
                var recordOffset = this.fileStream.Position;
                using var writer = new BinaryWriter(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
                writer.Write((short)(status | 0b0100));
                writer.Flush();

                RemoveFromDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                RemoveFromDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth, recordOffset);
                break;
            }
        }

        if (this.fileStream.Position >= this.fileStream.Length)
        {
            throw new ArgumentException($"#{id} record is not found.");
        }
    }

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id) =>
        this.ReadAllRecords((record, i) => record.Id == i, id).SingleOrDefault();

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByFirstName(string? firstName)
    {
        if (firstName != null && this.firstNameDictionary.TryGetValue(firstName.ToUpper(CultureInfo.InvariantCulture), out var results))
        {
            using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
            foreach (var recordOffset in results)
            {
                this.fileStream.Seek(recordOffset, SeekOrigin.Begin);
                var (status, record) = ReadRecord(reader);
                yield return record;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByLastName(string? lastName)
    {
        if (lastName != null && this.lastNameDictionary.TryGetValue(lastName.ToUpper(CultureInfo.InvariantCulture), out var results))
        {
            using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
            foreach (var recordOffset in results)
            {
                this.fileStream.Seek(recordOffset, SeekOrigin.Begin);
                var (status, record) = ReadRecord(reader);
                yield return record;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
    {
        if (DateTime.TryParse(dateOfBirthString, out var dateOfBirth) && this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out var results))
        {
            using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
            foreach (var recordOffset in results)
            {
                this.fileStream.Seek(recordOffset, SeekOrigin.Begin);
                var (status, record) = ReadRecord(reader);
                yield return record;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> Find(Dictionary<string, string> conditions) =>
        this.GetRecords().Where(record => CheckRecordSatisfiesConditions(record, conditions)).ToList();

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot() => new FileCabinetServiceSnapshot(this.GetRecords().ToArray());

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref Collection<string> errorsList)
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
                this.recordValidator.ValidateParameters(parameters);

                this.fileStream.Seek(0, SeekOrigin.Begin);
                using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

                while (this.fileStream.Position < this.fileStream.Length)
                {
                    var (status, recordRead) = ReadRecord(reader);
                    if ((status & 0b0100) == 0 && recordRead.Id == record.Id)
                    {
                        this.fileStream.Seek(-RecordSize, SeekOrigin.Current);
                        var recordOffset = this.fileStream.Position;
                        AddToDictionary(this.firstNameDictionary, parameters.FirstName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                        AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
                        AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, recordOffset);
                        break;
                    }
                }

                using var writer = new BinaryWriter(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
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
        this.fileStream.Seek(0, SeekOrigin.Begin);

        long writePosition = 0;
        long readPosition = 0;

        while (readPosition < this.fileStream.Length)
        {
            this.fileStream.Seek(readPosition, SeekOrigin.Begin);
            using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
            using var writer = new BinaryWriter(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

            var status = reader.ReadInt16();
            var recordBytes = reader.ReadBytes(RecordSize - 2);

            if ((status & 0b0100) == 0)
            {
                if (writePosition != readPosition)
                {
                    this.fileStream.Seek(writePosition, SeekOrigin.Begin);
                    writer.Write(status);
                    writer.Write(recordBytes);
                }

                writePosition += RecordSize;
            }

            readPosition += RecordSize;
        }

        this.fileStream.SetLength(writePosition);
    }

    /// <inheritdoc/>
    public void InsertRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        this.recordValidator.ValidateParameters(parameters);
        var recordOffset = this.fileStream.Length;

        this.fileStream.Seek(0, SeekOrigin.End);
        using var writer = new BinaryWriter(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);
        WriteRecord(id, parameters, writer);
        writer.Flush();

        AddToDictionary(this.firstNameDictionary, parameters!.FirstName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
        AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), recordOffset);
        AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, recordOffset);
    }

    private static char[] PadString(string? value, int length)
    {
        var padded = new char[length];
        var valueChars = (value ?? string.Empty).ToCharArray();
        Array.Copy(valueChars, padded, Math.Min(valueChars.Length, length));
        return padded;
    }

    private static (short, FileCabinetRecord) ReadRecord(BinaryReader reader)
    {
        var status = reader.ReadInt16();
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
        return (status, record);
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
        this.fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(this.fileStream, System.Text.Encoding.Unicode, leaveOpen: true);

        while (this.fileStream.Position < this.fileStream.Length)
        {
            var (status, record) = ReadRecord(reader);
            if ((status & 0b0100) == 0 && condition(record, checkString))
            {
                records.Add(record);
            }
        }

        return records.AsReadOnly();
    }

    /// <summary>
    /// Adds record to a dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to which the record should be added.</param>
    /// <param name="key">The key under which the record will be stored.</param>
    /// <param name="recordOffset">The record to be added to the dictionary.</param>
    /// <typeparam name="T">The type of the key in the dictionary.</typeparam>
    private static void AddToDictionary<T>(IDictionary<T, List<long>> dictionary, T key, long recordOffset)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = new List<long>();
            dictionary[key] = value;
        }

        value.Add(recordOffset);
    }

    /// <summary>
    /// Removes a record from the dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary from which the record should be removed.</param>
    /// <param name="key">The key under which the record is stored.</param>
    /// <param name="recordOffset">The record to be removed from the dictionary.</param>
    /// <typeparam name="T">The type of the key in the dictionary.</typeparam>
    private static void RemoveFromDictionary<T>(IDictionary<T, List<long>> dictionary, T key, long recordOffset)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            value.Remove(recordOffset);
            if (value.Count == 0)
            {
                dictionary.Remove(key);
            }
        }
    }

    private static bool CheckRecordSatisfiesConditions(FileCabinetRecord record, Dictionary<string, string> conditions)
    {
        foreach (var (field, value) in conditions)
        {
            var property = typeof(FileCabinetRecord).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException("No such field");
            }

            var recordValue = property.GetValue(record);
            var convertedValue = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);

            switch (recordValue)
            {
                case string recordStr:
                    if (!string.Equals(value, recordStr, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    break;

                case char recordChar when convertedValue is char convertedChar:
                    if (char.ToUpperInvariant(recordChar) != char.ToUpperInvariant(convertedChar))
                    {
                        return false;
                    }

                    break;
                default:
                    if (!Equals(recordValue, convertedValue))
                    {
                        return false;
                    }

                    break;
            }
        }

        return true;
    }
}