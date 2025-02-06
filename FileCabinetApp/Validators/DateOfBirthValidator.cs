namespace FileCabinetApp.Validators;

/// <summary>
/// Class represents date of birth validator.
/// </summary>
/// <param name="dateFrom">Min allowed date of birth.</param>
/// <param name="dateTo">Max allowed date of birth.</param>
public class DateOfBirthValidator(DateTime dateFrom, DateTime dateTo) : IRecordValidator
{
    private readonly DateTime dateFrom = dateFrom > DateTime.Today || dateFrom > dateTo
        ? throw new ArgumentException("Date from can't be more than current date or date to")
        : dateFrom;

    private readonly DateTime dateTo = dateTo > DateTime.Today ? throw new ArgumentException("Date can't be more than current date") : dateTo;

    /// <inheritdoc/>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(parameters.DateOfBirth);

        _ = parameters.DateOfBirth < this.dateFrom || parameters.DateOfBirth > this.dateTo
            ? throw new ArgumentException($"Date of birth can't be less than {this.dateFrom} or more than current date")
            : parameters.DateOfBirth;
    }
}