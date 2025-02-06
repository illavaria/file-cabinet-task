namespace FileCabinetApp.ValidationSettings;

/// <summary>
/// Class represents validation settings.
/// </summary>
public class ValidationSettings
{
    /// <summary>
    /// Gets or sets default validation settings.
    /// </summary>
    public DefaultSettings? Default { get; set; }

    /// <summary>
    /// Gets or sets custom validation settings.
    /// </summary>
    public CustomSettings? Custom { get; set; }
}