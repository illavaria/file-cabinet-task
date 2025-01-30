using System.Collections.ObjectModel;

namespace FileCabinetApp;

public class GenderValidator:IRecordValidator
{
    private readonly ReadOnlyCollection<char> allowedGenders;

    public GenderValidator(List<char> allowedGenders)
    {
        ArgumentNullException.ThrowIfNull(allowedGenders);
        this.allowedGenders = allowedGenders.AsReadOnly();
    }

    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        _ = allowedGenders.Contains(parameters.Gender)
            ? parameters.Gender
            : throw new ArgumentException(
                $"Gender must be one of the following: {string.Join(", ", this.allowedGenders)}");
    }
}