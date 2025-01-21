using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

// ReSharper disable InconsistentNaming
// ReSharper disable NullableWarningSuppressionIsUsed
namespace FileCabinetApp;

/// <summary>
/// Abstract class represents file cabinet service.
/// </summary>
public class FileCabinetMemoryService(IRecordValidator validator) : IFileCabinetService
{
    private readonly List<FileCabinetRecord> list = new ();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
    private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

    /// <summary>
    /// Creates a new record.
    /// </summary>
    /// <param name="parameters">Record's parameters.</param>
    /// <returns>Id of the created record.</returns>
    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        validator.ValidateParameters(parameters);

        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = parameters.FirstName,
            LastName = parameters.LastName,
            DateOfBirth = parameters.DateOfBirth,
            NumberOfChildren = parameters.NumberOfChildren,
            YearIncome = parameters.YearIncome,
            Gender = parameters.Gender,
        };

        this.list.Add(record);
        AddToDictionary(this.firstNameDictionary, parameters.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.lastNameDictionary, parameters.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.dateOfBirthDictionary, parameters.DateOfBirth, record);

        return record.Id;
    }

    /// <summary>
    /// Returns all records.
    /// </summary>
    /// <returns>Records list.</returns>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords() => new (this.list);

    /// <summary>
    /// Gets the number of records.
    /// </summary>
    /// <returns>Number of records.</returns>
    public int GetStat() => this.list.Count;

    /// <summary>
    /// Edits the record.
    /// </summary>
    /// <param name="id">Record Id.</param>
    /// <param name="parameters">Record's parameters.</param>
    /// <exception cref="ArgumentException">Thrown if record with such id doesn't exist.</exception>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        var record = this.FindById(id) ?? throw new ArgumentException($"#{id} record is not found.");
        validator.ValidateParameters(parameters);

        if (!string.Equals(record.FirstName, parameters.FirstName, StringComparison.OrdinalIgnoreCase))
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

        record.FirstName = parameters.FirstName;
        record.LastName = parameters.LastName;
        record.NumberOfChildren = parameters.NumberOfChildren;
        record.YearIncome = parameters.YearIncome;
        record.Gender = parameters.Gender;
    }

    /// <summary>
    /// Finds record by its id.
    /// </summary>
    /// <param name="id">Record Id.</param>
    /// <returns>Record if found, otherwise null.</returns>
    public FileCabinetRecord? FindById(int id) => this.list.Find(x => x.Id == id);

    /// <summary>
    /// Find records by first name.
    /// </summary>
    /// <param name="firstName">First name to search.</param>
    /// <returns>List of records with the matched first name.</returns>
    public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string? firstName)
        => firstName != null &&
           this.firstNameDictionary.TryGetValue(firstName.ToUpper(CultureInfo.InvariantCulture), out var results)
            ? new ReadOnlyCollection<FileCabinetRecord>(results)
            : new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());

    /// <summary>
    /// Find records by last name.
    /// </summary>
    /// <param name="lastName">Last name to search.</param>
    /// <returns>List of records with the matched last name.</returns>
    public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string? lastName)
        => lastName != null &&
           this.lastNameDictionary.TryGetValue(lastName.ToUpper(CultureInfo.InvariantCulture), out var results)
            ? new ReadOnlyCollection<FileCabinetRecord>(results)
            : new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());

    /// <summary>
    /// Find records by date of birth.
    /// </summary>
    /// <param name="dateOfBirthString">String representing date of birth.</param>
    /// <returns>List of records with the matched date of birth.</returns>
    public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
    {
        if (!DateTime.TryParse(dateOfBirthString, out var dateOfBirth))
        {
            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        return this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out var results)
            ? new ReadOnlyCollection<FileCabinetRecord>(results)
            : new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
    }

    public FileCabinetServiceSnapshot MakeSnapshot() => new FileCabinetServiceSnapshot(this.list.ToArray());

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
}