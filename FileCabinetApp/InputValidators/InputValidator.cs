using FileCabinetApp.ValidationSettings;

namespace FileCabinetApp.InputValidators;

/// <inheritdoc />
public class InputValidator(IBaseSettings validationSettings) : IRecordConsoleInputValidator
{
    private const string CorrectParameterText = "Correct parameter";
    private IBaseSettings validationSettings = validationSettings ?? throw new ArgumentNullException(nameof(validationSettings));

    /// <inheritdoc />
    public Tuple<bool, string> FirstNameValidator(string? firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return new Tuple<bool, string>(false, "First name must have symbols other than white spase");
        }

        return firstName.Length < this.validationSettings.FirstName.Min ||
               firstName.Length > this.validationSettings.FirstName.Max
            ? new Tuple<bool, string>(false, $"First name must be in range of {this.validationSettings.FirstName.Min} to {this.validationSettings.FirstName.Max} symbols")
            : new Tuple<bool, string>(true, CorrectParameterText);
    }

    /// <inheritdoc />
    public Tuple<bool, string> LastNameValidator(string? lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return new Tuple<bool, string>(false, "Last name must have symbols other than white spase");
        }

        if (lastName.Length < this.validationSettings.LastName.Min ||
            lastName.Length > this.validationSettings.LastName.Max)
        {
            return new Tuple<bool, string>(false,
                $"Last name must be in range of {this.validationSettings.LastName.Min} to {this.validationSettings.LastName.Max} symbols");
        }

        return lastName.Any(char.IsDigit)
            ? new Tuple<bool, string>(false, "Last name can't contain digits")
            : new Tuple<bool, string>(true, CorrectParameterText);
    }

    /// <inheritdoc />
    public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth) =>
        dateOfBirth < this.validationSettings.DateOfBirth.DateFrom ||
        dateOfBirth > this.validationSettings.DateOfBirth.DateTo
            ? new Tuple<bool, string>(false, $"Date of birth can't be less than {this.validationSettings.DateOfBirth.DateFrom} or more than {this.validationSettings.DateOfBirth.DateTo}")
            : new Tuple<bool, string>(true, CorrectParameterText);

    /// <inheritdoc />
    public Tuple<bool, string> NumberOfChildrenValidator(short numberOfChildren) =>
        numberOfChildren < this.validationSettings.NumberOfChildren.Min ||
        numberOfChildren > this.validationSettings.NumberOfChildren.Max
            ? new Tuple<bool, string>(false, $"Number of children can't be less than {this.validationSettings.NumberOfChildren.Min} or more than {this.validationSettings.NumberOfChildren.Max}")
            : new Tuple<bool, string>(true, CorrectParameterText);

    /// <inheritdoc />
    public Tuple<bool, string> YearIncomeValidator(decimal yearIncome) =>
        yearIncome < this.validationSettings.YearIncome.Min || yearIncome > this.validationSettings.YearIncome.Max
            ? new Tuple<bool, string>(false, $"Annual income can't be less than {this.validationSettings.YearIncome.Min} or more than {this.validationSettings.YearIncome.Max}")
            : new Tuple<bool, string>(true, CorrectParameterText);

    /// <inheritdoc />
    public Tuple<bool, string> GenderValidator(char gender) =>
        this.validationSettings.Gender.Allowed.Contains(gender)
            ? new Tuple<bool, string>(true, CorrectParameterText)
            : new Tuple<bool, string>(false, $"Gender must be one of the following: {string.Join(", ", this.validationSettings.Gender.Allowed)}");
}