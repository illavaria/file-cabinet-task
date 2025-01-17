using System.Globalization;
using System.Security.Cryptography;

// ReSharper disable NullableWarningSuppressionIsUsed
namespace FileCabinetApp;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new ();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
    private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

    public int CreateRecord(string? firstName, string? lastName, DateTime dateOfBirth, short numberOfChildren, decimal yearIncome, char gender)
    {
        ValidateRecordParams(firstName, lastName, dateOfBirth, numberOfChildren, yearIncome, gender);

        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            NumberOfChildren = numberOfChildren,
            YearIncome = yearIncome,
            Gender = gender,
        };

        this.list.Add(record);
        AddToDictionary(this.firstNameDictionary, firstName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.lastNameDictionary, lastName!.ToUpper(CultureInfo.InvariantCulture), record);
        AddToDictionary(this.dateOfBirthDictionary, dateOfBirth, record);

        return record.Id;
    }

    public FileCabinetRecord[] GetRecords()
    {
        var copyList = new FileCabinetRecord[this.list.Count];
        this.list.CopyTo(copyList);
        return copyList;
    }

    public int GetStat() => this.list.Count;

    public void EditRecord(int id, string? firstName, string? lastName, DateTime dateOfBirth, short numberOfChildren,
        decimal yearIncome, char gender)
    {
        var record = this.FindById(id) ?? throw new ArgumentException($"#{id} record is not found.");
        ValidateRecordParams(firstName, lastName, dateOfBirth, numberOfChildren, yearIncome, gender);

        if (!string.Equals(record.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
        {
            RemoveFromDictionary(this.firstNameDictionary, record.FirstName!.ToUpper(CultureInfo.InvariantCulture), record);
            AddToDictionary(this.firstNameDictionary, firstName!.ToUpper(CultureInfo.InvariantCulture), record);
        }

        if (!string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
        {
            RemoveFromDictionary(this.lastNameDictionary, record.LastName!.ToUpper(CultureInfo.InvariantCulture), record);
            AddToDictionary(this.lastNameDictionary, lastName!.ToUpper(CultureInfo.InvariantCulture), record);
        }

        if (record.DateOfBirth != dateOfBirth)
        {
            RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth, record);
            AddToDictionary(this.dateOfBirthDictionary, dateOfBirth, record);
            record.DateOfBirth = dateOfBirth;
        }

        record.FirstName = firstName;
        record.LastName = lastName;
        record.NumberOfChildren = numberOfChildren;
        record.YearIncome = yearIncome;
        record.Gender = gender;
    }

    public FileCabinetRecord? FindById(int id) => this.list.Find(x => x.Id == id);

    public FileCabinetRecord[] FindByFirstName(string? firstName)
        => firstName != null && this.firstNameDictionary.TryGetValue(firstName.ToUpper(CultureInfo.InvariantCulture), out var results)
            ? results.ToArray()
            : Array.Empty<FileCabinetRecord>();

    public FileCabinetRecord[] FindByLastName(string? lastName)
        => lastName != null && this.lastNameDictionary.TryGetValue(lastName.ToUpper(CultureInfo.InvariantCulture), out var results)
        ? results.ToArray()
        : Array.Empty<FileCabinetRecord>();

    public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirthString)
    {
        if (!DateTime.TryParse(dateOfBirthString, out var dateOfBirth))
        {
            return Array.Empty<FileCabinetRecord>();
        }

        return this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out var records)
            ? records.ToArray()
            : Array.Empty<FileCabinetRecord>();
    }


    private static void ValidateRecordParams(string? firstName, string? lastName, DateTime dateOfBirth, short numberOfChildren, decimal yearIncome, char gender)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        _ = firstName.Length is < 2 or > 60
            ? throw new ArgumentException("First name must be in range of 2 to 60 symbols")
            : firstName;
        _ = lastName.Length is < 2 or > 60
            ? throw new ArgumentException("Last name must be in range of 2 to 60 symbols")
            : lastName;
        _ = dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today
            ? throw new ArgumentException("Date of birth can't be less than 01-Jan-1950 or more than current date")
            : dateOfBirth;
        _ = numberOfChildren is < 0 or > 10
            ? throw new ArgumentException("Number of children can't be less than 0 or more than 10")
            : numberOfChildren;
        _ = yearIncome is < 0 or > 250_000
            ? throw new ArgumentException("Annual income can't be less than 0 or more than 250,000")
            : yearIncome;
        _ = gender != 'M' && gender != 'F' && gender != 'N'
            ? throw new ArgumentException(
                "Gender has to be one of the following: M - male, F - female, N - not answered")
            : gender;
    }

    private static void AddToDictionary<T>(IDictionary<T, List<FileCabinetRecord>> dictionary, T key, FileCabinetRecord record)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = new List<FileCabinetRecord>();
            dictionary[key] = value;
        }

        value.Add(record);
    }

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