namespace FileCabinetApp;

public interface IRecordConsoleInputValidator
{

    /// <summary>
    /// Validates first name.
    /// </summary>
    /// <param name="firstName">First name for validation.</param>
    /// <returns>Tuple with bool value and string to represent validation error.</returns>
    public Tuple<bool, string> FirstNameValidator(string? firstName);

    /// <summary>
    /// Validates last name.
    /// </summary>
    /// <param name="lastName">Last name for validation.</param>
    /// <returns>Tuple with bool value and string to represent validation error.</returns>
    public Tuple<bool, string> LastNameValidator(string? lastName);

    /// <summary>
    /// Validates date of birth.
    /// </summary>
    /// <param name="dateOfBirth">Date of birth for validation.</param>
    /// <returns>Tuple with bool value and string to represent validation error.</returns>
    public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth);

    /// <summary>
    /// Validates number of children.
    /// </summary>
    /// <param name="numberOfChildren">Number of children for validation.</param>
    /// <returns>Tuple with bool value and string to represent validation error.</returns>
    public Tuple<bool, string> NumberOfChildrenValidator(short numberOfChildren);

    /// <summary>
    /// Validates year income.
    /// </summary>
    /// <param name="yearIncome">Year income for validation.</param>
    /// <returns>Tuple with bool value and string to represent validation error.</returns>
    public Tuple<bool, string> YearIncomeValidator(decimal yearIncome);

    /// <summary>
    /// Validates gender.
    /// </summary>
    /// <param name="gender">Gender for validation.</param>
    /// <returns>Tuple with bool value and string to represent validation error.</returns>
    public Tuple<bool, string> GenderValidator(char gender);
}