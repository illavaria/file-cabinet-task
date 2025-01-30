namespace FileCabinetApp;

public static class ValidatorExtensions
{
    public static IRecordValidator CreateDefault(this ValidatorBuilder builder)
    {
        builder.ValidateFirstName(2, 60)
            .ValidateLastName(2, 60)
            .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Today)
            .ValidateNumberOfChildren(0, 10)
            .ValidateYearIncrome(0, 250_000)
            .ValidateGender(['M', 'N', 'F']);
        return builder.Create();
    }

    public static IRecordValidator CreateCustom(this ValidatorBuilder builder)
    {
        builder.ValidateFirstName(2, 40)
            .ValidateLastName(2, 40)
            .ValidateDateOfBirth(new DateTime(1940, 1, 1), DateTime.Today)
            .ValidateNumberOfChildren(0, 5)
            .ValidateNumberOfChildrenAndAge(16)
            .ValidateYearIncrome(0, 200_000)
            .ValidateGender(['M', 'N', 'F']);
        return builder.Create();
    }
}