namespace FileCabinetApp.ValidationSettings;

/// <summary>
/// Class represents date validation settings.
/// </summary>
public class RangeDateSettings
{
    /// <summary>
    /// Gets or sets min allowed date.
    /// </summary>
    public DateTime DateFrom { get; set; }

    /// <summary>
    /// Gets or sets max allowed date.
    /// </summary>
    public DateTime DateTo { get; set; }
}