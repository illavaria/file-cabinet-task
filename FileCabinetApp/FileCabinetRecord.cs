namespace FileCabinetApp;

public class FileCabinetRecord
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public short NumberOfChildren { get; set; }

    public decimal YearIncome { get; set; }

    public char Gender { get; set; }

    public override string ToString() =>
    $"{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth:yyyy-MMM-dd}, {this.NumberOfChildren}, {this.YearIncome}, {this.Gender}";
}