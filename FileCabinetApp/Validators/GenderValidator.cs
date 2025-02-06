using System.Collections.ObjectModel;

namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents gender validator.
/// </summary>
/// <param name="allowedGenders">List of allowed genders.</param>
public class GenderValidator(List<char> allowedGenders) : IRecordValidator
{
    private readonly ReadOnlyCollection<char> allowedGenders = allowedGenders.AsReadOnly() ?? throw new ArgumentNullException(nameof(allowedGenders));

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        _ = this.allowedGenders.Contains(char.ToUpperInvariant(parameters.Gender))
            ? parameters.Gender
            : throw new ArgumentException(
                $"Gender must be one of the following: {string.Join(", ", this.allowedGenders)}");
    }
}