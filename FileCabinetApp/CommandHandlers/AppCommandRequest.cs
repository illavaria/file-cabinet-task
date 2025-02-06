namespace FileCabinetApp.CommandHandlers;

/// <summary>
/// Class represents app command request.
/// </summary>
public class AppCommandRequest
{
    /// <summary>
    /// Gets or sets command name.
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// Gets or sets command parameters.
    /// </summary>
    public string? Parameters { get; set; }
}