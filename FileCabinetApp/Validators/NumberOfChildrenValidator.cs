namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents number of children validator.
/// </summary>
/// <param name="min">Min number of children allowed.</param>
/// <param name="max">Max number of children allowed.</param>
public class NumberOfChildrenValidator(short min, short max) : IRecordValidator
{
    private readonly short min = min < 0 ? throw new ArgumentOutOfRangeException(nameof(min)) : min;
    private readonly short max = max < 0 || max < min ? throw new ArgumentOutOfRangeException(nameof(max)) : max;

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        _ = parameters.NumberOfChildren > this.max || parameters.NumberOfChildren < this.min
            ? throw new ArgumentException($"Number of children must be in range {this.min}-{this.max}, but was {parameters.NumberOfChildren}")
            : parameters.NumberOfChildren;
    }
}