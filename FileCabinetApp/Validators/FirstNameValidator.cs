namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents first name validator.
/// </summary>
/// <param name="minLength">Min length allowed for first name.</param>
/// <param name="maxLength">Max length allowed for first name.</param>
public class FirstNameValidator(int minLength, int maxLength) : IRecordValidator
{
    private readonly int minLength = minLength < 0 ? throw new ArgumentOutOfRangeException(nameof(minLength)) : minLength;
    private readonly int maxLength = maxLength < 0 || maxLength < minLength
        ? throw new ArgumentOutOfRangeException(nameof(maxLength))
        : maxLength;

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameters.FirstName);

        if (parameters.FirstName.Length < this.minLength || parameters.FirstName.Length > this.maxLength)
        {
            throw new ArgumentException(nameof(parameters.LastName), $"First name must be in range of {this.minLength} to {this.maxLength} symbols");
        }
    }
}