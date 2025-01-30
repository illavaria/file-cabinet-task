namespace FileCabinetApp;

public class FirstNameValidator:IRecordValidator
{ 
    private readonly int minLength;
    private readonly int maxLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
    /// </summary>
    /// <param name="minLength"></param>
    /// <param name="maxLength"></param>
    /// <param name="maxLangth"></param>
    public FirstNameValidator(int minLength, int maxLength)
    {
        this.minLength = minLength < 0 ? throw new ArgumentOutOfRangeException(nameof(minLength)) : minLength;
        this.maxLength = maxLength < 0 || maxLength < minLength
            ? throw new ArgumentOutOfRangeException(nameof(maxLength))
            : maxLength;
    }

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