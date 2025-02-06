namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents year income validator.
/// </summary>
/// <param name="min">Min allowed value.</param>
/// <param name="max">Max allowed value.</param>
public class YearIncomeValidator(decimal min, decimal max) : IRecordValidator
{
    private readonly decimal min = min < 0 ? throw new ArgumentOutOfRangeException(nameof(min)) : min;
    private readonly decimal max = max < 0 || max < min ? throw new ArgumentOutOfRangeException(nameof(max)) : max;

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        _ = parameters.YearIncome > this.max || parameters.YearIncome < this.min
            ? throw new ArgumentException($"Year income must be in range {this.min}-{this.max}, but was {parameters.YearIncome}")
            : parameters.NumberOfChildren;
    }
}