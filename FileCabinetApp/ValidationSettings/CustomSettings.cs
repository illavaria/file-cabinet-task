namespace FileCabinetApp.ValidationSettings;

/// <summary>
/// Class represents custom validation settings.
/// </summary>
public class CustomSettings : IBaseSettings
{
    /// <inheritdoc/>
    public RangeSettings FirstName { get; set; }

    /// <inheritdoc/>
    public RangeSettings LastName { get; set; }

    /// <inheritdoc/>
    public RangeDateSettings DateOfBirth { get; set; }

    /// <inheritdoc/>
    public RangeSettings NumberOfChildren { get; set; }

    /// <summary>
    /// Gets or sets the min age allowed to have children.
    /// </summary>
    public int MinAgeToHaveChildren { get; set; }

    /// <inheritdoc/>
    public RangeSettings YearIncome { get; set; }

    /// <inheritdoc/>
    public GenderSettings Gender { get; set; }
}