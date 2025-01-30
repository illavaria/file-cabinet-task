namespace FileCabinetApp;

public class CompositeValidator : IRecordValidator
{
    private List<IRecordValidator> validators;

    public CompositeValidator(IEnumerable<IRecordValidator> validators)
    {
        this.validators = validators.ToList();
    }

    public void ValidateParameters(FileCabinetRecordsParameters parameters)
    {
        foreach (var validator in this.validators)
        {
            validator.ValidateParameters(parameters);
        }
    }
}