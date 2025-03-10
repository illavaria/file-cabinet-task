namespace FileCabinetApp;

/// <summary>
/// Class represents records in file cabinet.
/// </summary>
public class FileCabinetRecord
{
    /// <summary>
    /// Gets or sets record's id.
    /// </summary>
    public int Id { get; set; }

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

    /// <summary>
    /// Creates string representation of the record.
    /// </summary>
    /// <returns>String representation of the record.</returns>
    public override string ToString() =>
        $"{this.Id}, {this.FirstName}, {this.LastName}, {this.DateOfBirth:yyyy-MMM-dd}, {this.NumberOfChildren}, {this.YearIncome}, {this.Gender}";
}