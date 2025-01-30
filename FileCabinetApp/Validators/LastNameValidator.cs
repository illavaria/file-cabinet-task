namespace FileCabinetApp;

public class LastNameValidator: IRecordValidator
{
    private readonly int minLength;
    private readonly int maxLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
    /// </summary>
    /// <param name="minLength"></param>
    /// <param name="maxLength"></param>
    /// <param name="maxLangth"></param>
    public LastNameValidator(int minLength, int maxLength)
    {
        this.minLength = minLength < 0 ? throw new ArgumentOutOfRangeException(nameof(minLength)) : minLength;
        this.maxLength = maxLength < 0 || maxLength < minLength
            ? throw new ArgumentOutOfRangeException(nameof(maxLength))
            : maxLength;
    }

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