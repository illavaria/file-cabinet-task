namespace FileCabinetApp.Validators;

/// <summary>
/// Interface for validating record's parameters.
/// </summary>
public interface IRecordValidator
{
    /// <summary>
    /// Validates all record's parameters.
    /// </summary>
    /// <param name="parameters">Record's parameters for validation.</param>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters);
}