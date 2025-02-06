using Bogus;
using FileCabinetApp;
using FileCabinetApp.ValidationSettings;

namespace FileCabinetGenerator;

public class FileCabinetRecordGenerator
{
    private IBaseSettings configuration;
    
    public FileCabinetRecordGenerator(IBaseSettings configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        this.configuration = configuration;
    }

    public List<FileCabinetRecord> GenerateRecords(int amount, int startId)
    {
        List<FileCabinetRecord> records = new();
        var random = new Randomizer();
        var userFaker = new Faker<FileCabinetRecord>()
            .CustomInstantiator(f => new FileCabinetRecord())
            .RuleFor(i => i.Id, f => startId++)
            .RuleFor(i => i.Gender, f => this.configuration.Gender.Allowed[f.Random.Int(0, this.configuration.Gender.Allowed.Count - 1)])
            .RuleFor(i => i.FirstName,
                (f, i) => i.Gender switch
                {
                    'M' => f.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                    'F' => f.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                    _ => f.Name.FirstName()
                })
            .RuleFor(i => i.LastName, f => f.Name.LastName())
            .RuleFor(i => i.DateOfBirth, f => f.Date.Between(this.configuration.DateOfBirth.DateFrom, DateTime.Now))
            .RuleFor(i => i.NumberOfChildren, f => f.Random.Short((short)this.configuration.NumberOfChildren.Min, (short)this.configuration.NumberOfChildren.Max))
            .RuleFor(i => i.YearIncome, f => Math.Round(f.Random.Decimal(this.configuration.YearIncome.Min, this.configuration.YearIncome.Max), 2));

        records.AddRange(userFaker.Generate(amount));

        return records;
    }
}