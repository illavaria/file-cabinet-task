namespace FileCabinetApp;

public class DateOfBirthValidator:IRecordValidator
{
    private readonly DateTime dateFrom;//(1950, 1, 1)
    private readonly DateTime dateTo;

    public DateOfBirthValidator(DateTime dateFrom, DateTime dateTo)
    {
        this.dateFrom = dateFrom > DateTime.Today || dateFrom > dateTo
            ? throw new ArgumentException("Date from can't be more than current date or date to")
            : dateFrom;
        this.dateTo = dateTo > DateTime.Today ? throw new ArgumentException("Date can't be more than current date") : dateTo;
    }

    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(parameters.DateOfBirth);

        _ = parameters.DateOfBirth < this.dateFrom || parameters.DateOfBirth > this.dateTo
            ? throw new ArgumentException($"Date of birth can't be less than {this.dateFrom} or more than current date")
            : parameters
                .DateOfBirth;
    }
}