using System.Collections.ObjectModel;
using FileCabinetApp.ValidationSettings;

namespace FileCabinetApp.Validators;

/// <summary>
/// Validator builder.
/// </summary>
public class ValidatorBuilder
{
    private List<IRecordValidator> validators = new ();

    /// <summary>
    /// Creates default validator.
    /// </summary>
    /// <param name="validationSettings">Validation settings.</param>
    /// <returns>Record validator.</returns>
    public IRecordValidator CreateDefault(DefaultSettings validationSettings)
    {
        ArgumentNullException.ThrowIfNull(validationSettings);
        ArgumentNullException.ThrowIfNull(validationSettings.FirstName);
        ArgumentNullException.ThrowIfNull(validationSettings.LastName);
        ArgumentNullException.ThrowIfNull(validationSettings.DateOfBirth);
        ArgumentNullException.ThrowIfNull(validationSettings.NumberOfChildren);
        ArgumentNullException.ThrowIfNull(validationSettings.YearIncome);
        ArgumentNullException.ThrowIfNull(validationSettings.Gender);
        this.ValidateFirstName(validationSettings.FirstName.Min, validationSettings.FirstName.Max);
        this.ValidateLastName(validationSettings.LastName.Min, validationSettings.LastName.Max);
        this.ValidateDateOfBirth(validationSettings.DateOfBirth.DateFrom, DateTime.Today);
        this.ValidateNumberOfChildren((short)validationSettings.NumberOfChildren.Min, (short)validationSettings.NumberOfChildren.Max);
        this.ValidateYearIncome(validationSettings.YearIncome.Min, validationSettings.YearIncome.Max);
        this.ValidateGender(validationSettings.Gender.Allowed);
        return this.Create();
    }

    /// <summary>
    /// Creates custom validator.
    /// </summary>
    /// <param name="validationSettings">Validation settings.</param>
    /// <returns>Record validator.</returns>
    public IRecordValidator CreateCustom(CustomSettings validationSettings)
    {
        ArgumentNullException.ThrowIfNull(validationSettings);
        ArgumentNullException.ThrowIfNull(validationSettings.FirstName);
        ArgumentNullException.ThrowIfNull(validationSettings.LastName);
        ArgumentNullException.ThrowIfNull(validationSettings.DateOfBirth);
        ArgumentNullException.ThrowIfNull(validationSettings.NumberOfChildren);
        ArgumentNullException.ThrowIfNull(validationSettings.YearIncome);
        ArgumentNullException.ThrowIfNull(validationSettings.Gender);
        this.ValidateFirstName(validationSettings.FirstName.Min, validationSettings.FirstName.Max);
        this.ValidateLastName(validationSettings.LastName.Min, validationSettings.LastName.Max);
        this.ValidateDateOfBirth(validationSettings.DateOfBirth.DateFrom, DateTime.Today);
        this.ValidateNumberOfChildren((short)validationSettings.NumberOfChildren.Min, (short)validationSettings.NumberOfChildren.Max);
        this.ValidateNumberOfChildrenAndAge(validationSettings.MinAgeToHaveChildren);
        this.ValidateYearIncome(validationSettings.YearIncome.Min, validationSettings.YearIncome.Max);
        this.ValidateGender(validationSettings.Gender.Allowed);
        return this.Create();
    }

    private void ValidateFirstName(int minLength, int maxLength)
    {
        this.validators.Add(new FirstNameValidator(minLength, maxLength));
    }

    private void ValidateLastName(int minLength, int maxLength)
    {
        this.validators.Add(new LastNameValidator(minLength, maxLength));
    }

    private void ValidateDateOfBirth(DateTime dateFrom, DateTime dateTo)
    {
        this.validators.Add(new DateOfBirthValidator(dateFrom, dateTo));
    }

    private void ValidateNumberOfChildren(short min, short max)
    {
        this.validators.Add(new NumberOfChildrenValidator(min, max));
    }

    private void ValidateNumberOfChildrenAndAge(int minAge)
    {
        this.validators.Add(new NumberOfChildrenAndAgeValidator(minAge));
    }

    private void ValidateYearIncome(decimal min, decimal max)
    {
        this.validators.Add(new YearIncomeValidator(min, max));
    }

    private void ValidateGender(List<char> allowedGenders)
    {
        this.validators.Add(new GenderValidator(allowedGenders));
    }

    private IRecordValidator Create()
    {
        return new CompositeValidator(this.validators);
    }
}