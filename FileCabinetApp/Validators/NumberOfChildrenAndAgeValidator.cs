namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents min age to have children validator.
/// </summary>
/// <param name="minAge">Min age allowed to have children.</param>
public class NumberOfChildrenAndAgeValidator(int minAge) : IRecordValidator
{
    private readonly int minAge = minAge < 0 ? throw new ArgumentOutOfRangeException(nameof(minAge)) : minAge;

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        _ = DateTime.Today.Year - parameters.DateOfBirth.Year < this.minAge && parameters.NumberOfChildren > 0
            ? throw new ArgumentException("Person is too young to have children")
            : parameters.NumberOfChildren;
    }
}