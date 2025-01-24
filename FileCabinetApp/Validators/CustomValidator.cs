namespace FileCabinetApp;

/// <inheritdoc />
public class CustomValidator : IRecordValidator
{
    private readonly int minNameLength = 2;
    private readonly int maxNameLength = 40;
    private readonly string correctParameterText = "Correct parameter";

    /// <summary>
    /// Validates the records parameters.
    /// </summary>
    /// <param name="parameters">Record's parameters for validation.</param>
    /// <exception cref="ArgumentException">Thrown if:
    /// - First name or last name is null, empty, whitespace, or has an invalid length (less than 2 or more than 40).
    /// - Last name has digits.
    /// - Date of birth is outside the range January 1, 1940 - today's date.
    /// - Number of children is less than 0 or greater than 5.
    /// - Number of children is more than 0 and record's age is less than 16.
    /// - Annual income is less than 0 or greater than 200,000.
    /// - Gender is not one of the allowed values ('M', 'F', 'N').
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown if parameters are null.</exception>
    public void ValidateParameters(FileCabinetRecordsParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(parameters);

        ValidateParam(parameters.FirstName, this.FirstNameValidator);
        ValidateParam(parameters.LastName, this.LastNameValidator);
        ValidateParam(parameters.DateOfBirth, this.DateOfBirthValidator);
        ValidateParam(parameters.NumberOfChildren, this.NumberOfChildrenValidator);
        ValidateParam(parameters.YearIncome, this.YearIncomeValidator);
        ValidateParam(parameters.Gender, this.GenderValidator);

        _ = DateTime.Today.Year - parameters.DateOfBirth.Year < 16 && parameters.NumberOfChildren > 0
            ? throw new ArgumentException("Person is too young to have children")
            : parameters.NumberOfChildren;

        static void ValidateParam<T>(T param, Func<T, Tuple<bool, string>> validator)
        {
            var validationResult = validator(param);
            if (!validationResult.Item1)
            {
                throw new ArgumentException(validationResult.Item2);
            }
        }
    }

    /// <inheritdoc />
    public Tuple<bool, string> FirstNameValidator(string? firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return new Tuple<bool, string>(false, "First name must have symbols other than white spase");
        }

        return firstName.Length < this.minNameLength || firstName.Length > this.maxNameLength
            ? new Tuple<bool, string>(false, $"First name must be in range of {this.minNameLength} to {this.maxNameLength} symbols")
            : new Tuple<bool, string>(true, this.correctParameterText);
    }

    /// <inheritdoc />
    public Tuple<bool, string> LastNameValidator(string? lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return new Tuple<bool, string>(false, "Last name must have symbols other than white spase");
        }

        if (lastName.Length < this.minNameLength || lastName.Length > this.maxNameLength)
        {
            return new Tuple<bool, string>(false, $"Last name must be in range of {this.minNameLength} to {this.maxNameLength} symbols");
        }

        return lastName.Any(char.IsDigit)
            ? new Tuple<bool, string>(false, "Last name can't contain digits")
            : new Tuple<bool, string>(true, this.correctParameterText);
    }

    /// <inheritdoc />
    public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth) =>
        dateOfBirth < new DateTime(1940, 1, 1) || dateOfBirth > DateTime.Today
            ? new Tuple<bool, string>(false, "Date of birth can't be less than 01-Jan-1940 or more than current date")
            : new Tuple<bool, string>(true, this.correctParameterText);

    /// <inheritdoc />
    public Tuple<bool, string> NumberOfChildrenValidator(short numberOfChildren) =>
        numberOfChildren is < 0 or > 5
            ? new Tuple<bool, string>(false, "Number of children can't be less than 0 or more than 5")
            : new Tuple<bool, string>(true, this.correctParameterText);

    /// <inheritdoc />
    public Tuple<bool, string> YearIncomeValidator(decimal yearIncome) =>
        yearIncome is < 0 or > 200_000
            ? new Tuple<bool, string>(false, "Annual income can't be less than 0 or more than 200,000")
            : new Tuple<bool, string>(true, this.correctParameterText);

    /// <inheritdoc />
    public Tuple<bool, string> GenderValidator(char gender) =>
        gender != 'M' && gender != 'F' && gender != 'N'
            ? new Tuple<bool, string>(false, "Gender has to be one of the following: M - male, F - female, N - not answered")
            : new Tuple<bool, string>(true, this.correctParameterText);
}