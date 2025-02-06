namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents composite validator.
/// </summary>
/// <param name="validators">Validators used for validation.</param>
public class CompositeValidator(IEnumerable<IRecordValidator> validators) : IRecordValidator
{
    private List<IRecordValidator> validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        foreach (var validator in this.validators)
        {
            validator.ValidateParameters(parameters);
        }
    }
}