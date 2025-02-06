#pragma warning disable CS8618
namespace FileCabinetApp.ValidationSettings;

/// <summary>
/// Class represents custom validation settings.
/// </summary>
public class DefaultSettings : IBaseSettings
{
    /// <inheritdoc/>
    public RangeSettings FirstName { get; set; }

    /// <inheritdoc/>
    public RangeSettings LastName { get; set; }

    /// <inheritdoc/>
    public RangeDateSettings DateOfBirth { get; set; }

    /// <inheritdoc/>
    public RangeSettings NumberOfChildren { get; set; }

    /// <inheritdoc/>
    public RangeSettings YearIncome { get; set; }

    /// <inheritdoc/>
    public GenderSettings Gender { get; set; }
}