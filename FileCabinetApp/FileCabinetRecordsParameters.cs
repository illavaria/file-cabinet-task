namespace FileCabinetApp;

/// <summary>
/// Represents parameters required for creating a FileCabinetRecord.
/// </summary>
public class FileCabinetRecordsParameters
{
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