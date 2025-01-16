namespace FileCabinetApp;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth,  short numberOfChildren, decimal yearIncome, char gender)
    {
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
}