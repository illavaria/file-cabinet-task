namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents last name validator.
/// </summary>
/// <param name="minLength">Min length allowed for last name.</param>
/// <param name="maxLength">Max length allowed for last name.</param>
public class LastNameValidator(int minLength, int maxLength) : IRecordValidator
{
    private readonly int minLength = minLength < 0 ? throw new ArgumentOutOfRangeException(nameof(minLength)) : minLength;
    private readonly int maxLength = maxLength < 0 || maxLength < minLength
        ? throw new ArgumentOutOfRangeException(nameof(maxLength))
        : maxLength;

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameters.LastName);

        if (parameters.LastName.Length < this.minLength || parameters.LastName.Length > this.maxLength)
        {
            throw new ArgumentException(nameof(parameters.LastName), $"Last name must be in range of {this.minLength} to {this.maxLength} symbols");
        }
    }
}