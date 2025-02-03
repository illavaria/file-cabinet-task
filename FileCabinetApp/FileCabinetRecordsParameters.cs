namespace FileCabinetApp;

/// <summary>
/// Represents parameters required for creating a FileCabinetRecord.
/// </summary>
public class FileCabinetRecordsParameters
{
    
    public FileCabinetRecordsParameters(FileCabinetRecord record)
    {
        _ = record ?? throw new ArgumentNullException(nameof(record));
        this.FirstName = record.FirstName;
        this.LastName = record.LastName;
        this.DateOfBirth = record.DateOfBirth;
        this.NumberOfChildren = record.NumberOfChildren;
        this.YearIncome = record.YearIncome;
        this.Gender = record.Gender;
    }

    public FileCabinetRecordsParameters()
    {
    }

    /// <summary>
    /// Gets or sets first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets date of birth.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets number of children.
    /// </summary>
    public short NumberOfChildren { get; set; }

    /// <summary>
    /// Gets or sets year income.
    /// </summary>
    public decimal YearIncome { get; set; }

    /// <summary>
    /// Gets or sets gender.
    /// </summary>
    public char Gender { get; set; }
}