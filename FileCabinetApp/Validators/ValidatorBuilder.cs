using System.Globalization;
using FileCabinetApp.ValidationSettings;

namespace FileCabinetApp;

public class ValidatorBuilder
{
    private List<IRecordValidator> validators;

    public ValidatorBuilder()
    {
        this.validators = new List<IRecordValidator>();
    }

    public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
    {
        this.validators.Add(new FirstNameValidator(minLength, maxLength));
        return this;
    }
    
    public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
    {
        this.validators.Add(new LastNameValidator(minLength, maxLength));
        return this;
    }

    public ValidatorBuilder ValidateDateOfBirth(DateTime dateFrom, DateTime dateTo)
    {
        this.validators.Add(new DateOfBirthValidator(dateFrom, dateTo));
        return this;
    }

    public ValidatorBuilder ValidateNumberOfChildren(short min, short max)
    {
        this.validators.Add(new NumberOfChildrenValidator(min, max));
        return this;
    }

    public ValidatorBuilder ValidateNumberOfChildrenAndAge(int minAge)
    {
        this.validators.Add(new NumberOfChildrenAndAgeValidator(minAge));
        return this;
    }

    public ValidatorBuilder ValidateYearIncrome(decimal min, decimal max)
    {
        this.validators.Add(new YearIncomeValidator(min, max));
        return this;
    }

    public ValidatorBuilder ValidateGender(List<char> allowedGenders)
    {
        this.validators.Add(new GenderValidator(allowedGenders));
        return this;
    }

    public IRecordValidator Create()
    {
        return new CompositeValidator(this.validators);
    }

    public IRecordValidator CreateDefault(DefaultSettings validationSettings)
    {
        ArgumentNullException.ThrowIfNull(validationSettings);
        this.ValidateFirstName(validationSettings.FirstName.Min, validationSettings.FirstName.Max);
        this.ValidateLastName(validationSettings.LastName.Min, validationSettings.LastName.Max);
        this.ValidateDateOfBirth(validationSettings.DateOfBirth.DateFrom, DateTime.Today);
        this.ValidateNumberOfChildren((short)validationSettings.NumberOfChildren.Min, (short)validationSettings.NumberOfChildren.Max);
        this.ValidateYearIncrome(validationSettings.YearIncome.Min, validationSettings.YearIncome.Max);
        this.ValidateGender(validationSettings.Gender.Allowed);
        return this.Create();
    }

    public IRecordValidator CreateCustom(CustomSettings validationSettings) //add check for numbers in last name
    {
        ArgumentNullException.ThrowIfNull(validationSettings);
        this.ValidateFirstName(validationSettings.FirstName.Min, validationSettings.FirstName.Max);
        this.ValidateLastName(validationSettings.LastName.Min, validationSettings.LastName.Max);
        this.ValidateDateOfBirth(validationSettings.DateOfBirth.DateFrom, DateTime.Today);
        this.ValidateNumberOfChildren((short)validationSettings.NumberOfChildren.Min, (short)validationSettings.NumberOfChildren.Max);
        this.ValidateNumberOfChildrenAndAge(validationSettings.MinAgeToHaveChildren);
        this.ValidateYearIncrome(validationSettings.YearIncome.Min, validationSettings.YearIncome.Max);
        this.ValidateGender(validationSettings.Gender.Allowed);
        return this.Create();
    }
}