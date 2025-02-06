using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using FileCabinetApp.Validators;

// ReSharper disable InconsistentNaming
// ReSharper disable NullableWarningSuppressionIsUsed
namespace FileCabinetApp.FileCabinetServices;

/// <summary>
/// Class represents memory file cabinet service.
/// </summary>
public class FileCabinetMemoryService(IRecordValidator validator) : IFileCabinetService
{
    private readonly IRecordValidator validator = validator ?? throw new ArgumentNullException();
    private readonly List<FileCabinetRecord> list = new ();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
    private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

    private readonly Dictionary<string, List<FileCabinetRecord>> cache = new ();

    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        this.validator.ValidateParameters(parameters);

        var record = new FileCabinetRecord
        {
            Id = this.list.Count == 0 ? 1 : this.list.Max(x => x.Id) + 1,
            FirstName = parameters!.FirstName,
            LastName = parameters.LastName,
            DateOfBirth = parameters.DateOfBirth,
            NumberOfChildren = parameters.NumberOfChildren,
            YearIncome = parameters.YearIncome,
            Gender = parameters.Gender,
        };

        this.list.Add(record);
        this.cache.Clear();
        AddToDictionary(this.firstNameDictionary, parameters.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, record);

        return record.Id;
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords() => this.list.AsReadOnly();

    /// <inheritdoc/>
    public int GetNumberOfAllRecords() => this.list.Count;

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords() => 0;

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        var record = this.FindById(id) ?? throw new ArgumentException($"#{id} record is not found.");
        this.validator.ValidateParameters(parameters);

        if (!string.Equals(record.FirstName, parameters!.FirstName, StringComparison.OrdinalIgnoreCase))
        {
            RemoveFromDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
            AddToDictionary(this.firstNameDictionary, parameters.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
        }

        if (!string.Equals(record.LastName, parameters.LastName, StringComparison.OrdinalIgnoreCase))
        {
            RemoveFromDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
            AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
        }

        if (record.DateOfBirth != parameters.DateOfBirth)
        {
            RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth, record);
            AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, record);
            record.DateOfBirth = parameters.DateOfBirth;
        }

        this.cache.Clear();
        record.FirstName = parameters.FirstName;
        record.LastName = parameters.LastName;
        record.NumberOfChildren = parameters.NumberOfChildren;
        record.YearIncome = parameters.YearIncome;
        record.Gender = parameters.Gender;
    }

    /// <inheritdoc/>
    public void RemoveRecord(int id)
    {
        var record = this.FindById(id) ?? throw new ArgumentException($"#{id} record doesn't exist.");
        this.list.Remove(record);
        this.cache.Clear();
        RemoveFromDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
        RemoveFromDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
        RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth, record);
    }

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id) => this.list.Find(x => x.Id == id);

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByFirstName(string? firstName)
    {
        if (firstName != null && this.firstNameDictionary.TryGetValue(firstName.ToUpper(CultureInfo.InvariantCulture), out var results))
        {
            foreach (var result in results)
            {
                yield return result;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByLastName(string? lastName)
    {
        if (lastName != null && this.lastNameDictionary.TryGetValue(lastName.ToUpper(CultureInfo.InvariantCulture), out var results))
        {
            foreach (var result in results)
            {
                yield return result;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
    {
        if (DateTime.TryParse(dateOfBirthString, out var dateOfBirth) && this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out var results))
        {
            foreach (var result in results)
            {
                yield return result;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> Find(Dictionary<string, string> conditions)
    {
        _ = conditions ?? throw new ArgumentNullException(nameof(conditions));
        var hashKey = string.Join(',', conditions.Select(kv => $"{kv.Key}={kv.Value}"));
        if (this.cache.TryGetValue(hashKey, out var records))
        {
            return records;
        }

        records = this.list.Where(record => CheckRecordSatisfiesConditions(record, conditions)).ToList();
        this.cache.Add(hashKey, records);
        return records;
    }

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot() => new FileCabinetServiceSnapshot(this.list.ToArray());

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref Collection<string> errorsList)
    {
        _ = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        foreach (var record in snapshot.Records)
        {
            try
            {
                this.validator.ValidateParameters(new FileCabinetRecordsParameters
                {
                    FirstName = record.FirstName, LastName = record.LastName, DateOfBirth = record.DateOfBirth,
                    NumberOfChildren = record.NumberOfChildren, YearIncome = record.YearIncome, Gender = record.Gender,
                });
                var recordInList = this.FindById(record.Id);
                if (recordInList is not null)
                {
                    this.list.Remove(recordInList);
                }

                this.list.Add(record);
                this.cache.Clear();
                AddToDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
                AddToDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
                AddToDictionary(this.dateOfBirthDictionary, record.DateOfBirth, record);
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
        throw new NotSupportedException("Purge method is not available in memory service");
    }

    /// <inheritdoc/>
    public void InsertRecord(int id, FileCabinetRecordsParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        this.validator.ValidateParameters(parameters);
        if (this.FindById(id) is not null)
        {
            throw new ArgumentException($"Record with {id} already exists.");
        }

        var record = new FileCabinetRecord
        {
            Id = id,
            FirstName = parameters.FirstName,
            LastName = parameters.LastName,
            DateOfBirth = parameters.DateOfBirth,
            NumberOfChildren = parameters.NumberOfChildren,
            YearIncome = parameters.YearIncome,
            Gender = parameters.Gender,
        };

        this.list.Add(record);
        this.cache.Clear();

        AddToDictionary(this.firstNameDictionary, parameters.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, record);
    }

    /// <summary>
    /// Adds record to a dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to which the record should be added.</param>
    /// <param name="key">The key under which the record will be stored.</param>
    /// <param name="record">The record to be added to the dictionary.</param>
    /// <typeparam name="T">The type of the key in the dictionary.</typeparam>
    private static void AddToDictionary<T>(IDictionary<T, List<FileCabinetRecord>> dictionary, T key, FileCabinetRecord record)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = new List<FileCabinetRecord>();
            dictionary[key] = value;
        }

        value.Add(record);
    }

    /// <summary>
    /// Removes a record from the dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary from which the record should be removed.</param>
    /// <param name="key">The key under which the record is stored.</param>
    /// <param name="record">The record to be removed from the dictionary.</param>
    /// <typeparam name="T">The type of the key in the dictionary.</typeparam>
    private static void RemoveFromDictionary<T>(IDictionary<T, List<FileCabinetRecord>> dictionary, T key, FileCabinetRecord record)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            value.Remove(record);
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