namespace FileCabinetApp;

public class YearIncomeValidator:IRecordValidator
{
    private readonly decimal min;
    private readonly decimal max;

    public YearIncomeValidator(decimal min, decimal max)
    {
        this.min = min < 0 ? throw new ArgumentOutOfRangeException(nameof(this.min)) : min;
        this.max = max < 0 || max < min ? throw new ArgumentOutOfRangeException(nameof(max)) : max;
    }

    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentOutOfRangeException.ThrowIfLessThan(parameters.YearIncome, this.min);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(parameters.YearIncome, this.max);
    }
}