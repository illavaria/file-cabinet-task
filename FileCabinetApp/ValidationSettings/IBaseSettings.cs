namespace FileCabinetApp.ValidationSettings;

/// <summary>
/// Interface represents base validation settings.
/// </summary>
public interface IBaseSettings
{
    /// <summary>
    /// Gets or sets first name validation settings.
    /// </summary>
    public RangeSettings FirstName { get; set; }

    /// <summary>
    /// Gets or sets last name validation settings.
    /// </summary>
    public RangeSettings LastName { get; set; }

    /// <summary>
    /// Gets or sets date of birth validation settings.
    /// </summary>
    public RangeDateSettings DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets number of children validation settings.
    /// </summary>
    public RangeSettings NumberOfChildren { get; set; }

    /// <summary>
    /// Gets or sets year income validation settings.
    /// </summary>
    public RangeSettings YearIncome { get; set; }

    /// <summary>
    /// Gets or sets gender validation settings.
    /// </summary>
    public GenderSettings Gender { get; set; }
}