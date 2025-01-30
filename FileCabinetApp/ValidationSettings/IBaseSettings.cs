namespace FileCabinetApp.ValidationSettings;

public interface IBaseSettings
{
    public RangeSettings FirstName { get; set; }
    public RangeSettings LastName { get; set; }
    public RangeDateSettings DateOfBirth { get; set; }
    public RangeSettings NumberOfChildren { get; set; }
    public RangeSettings YearIncome { get; set; }
    public GenderSettings Gender { get; set; }
}