using System.Security.Cryptography;

namespace FileCabinetApp;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new ();

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

        return record.Id;
    }

    public FileCabinetRecord[] GetRecords()
    {
        var copyList = new FileCabinetRecord[this.list.Count];
        this.list.CopyTo(copyList);
        return copyList;
    }

    public int GetStat() => this.list.Count;

    public void EditRecord(int id, string? firstName, string? lastName, DateTime dateOfBirth, short numberOfChildren, decimal yearIncome, char gender)
    {
        var record = this.FindRecordById(id) ?? throw new ArgumentException($"#{id} record is not found.");
        ValidateRecordParams(firstName, lastName, dateOfBirth, numberOfChildren, yearIncome, gender);
        record.FirstName = firstName;
        record.LastName = lastName;
        record.DateOfBirth = dateOfBirth;
        record.NumberOfChildren = numberOfChildren;
        record.YearIncome = yearIncome;
        record.Gender = gender;
    }

    public FileCabinetRecord? FindRecordById(int id) => this.list.Find(x => x.Id == id);

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
}