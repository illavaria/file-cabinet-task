namespace FileCabinetApp;

public class NumberOfChildrenValidator : IRecordValidator
{
    private readonly short min;
    private readonly short max;

    public NumberOfChildrenValidator(short min, short max)
    {
        this.min = min < 0 ? throw new ArgumentOutOfRangeException(nameof(min)) : min;
        this.max = max < 0 || max < min ? throw new ArgumentOutOfRangeException(nameof(max)) : max;
    }

    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentOutOfRangeException.ThrowIfLessThan(parameters.NumberOfChildren, this.min);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(parameters.NumberOfChildren, this.max);
    }
}