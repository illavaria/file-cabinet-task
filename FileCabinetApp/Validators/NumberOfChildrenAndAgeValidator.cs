namespace FileCabinetApp;

public class NumberOfChildrenAndAgeValidator: IRecordValidator
{
    private readonly int minAge;

    public NumberOfChildrenAndAgeValidator(int minAge)
    {
        this.minAge = minAge < 0 ? throw new ArgumentOutOfRangeException(nameof(minAge)) : minAge;
    }

    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        _ = DateTime.Today.Year - parameters.DateOfBirth.Year < this.minAge && parameters.NumberOfChildren > 0
            ? throw new ArgumentException("Person is too young to have children")
            : parameters.NumberOfChildren;
    }
}