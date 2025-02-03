using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp;

public class DeleteCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "delete")
{
    protected override void HandleCore(string parameters)
    {
        var match = Regex.Match(parameters, @"where\s+(\w+)\s*=\s*'([^']+)'", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            Console.WriteLine("Invalid delete command format. Use: delete where <ColumnName> = '<Value>'");
            return;
        }

        var columnName = match.Groups[1].Value;
        var value = match.Groups[2].Value;
        if (columnName.Equals("id", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                this.fileCabinetService.RemoveRecord(int.Parse(value, CultureInfo.InvariantCulture));
                Console.WriteLine($"Record {value} is deleted.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            List<FileCabinetRecord> recordsToDelete = [];
            if (columnName.Equals("firstName", StringComparison.OrdinalIgnoreCase))
            {
                recordsToDelete = this.fileCabinetService.FindByFirstName(value).ToList();
            }
            else if (columnName.Equals("lastName", StringComparison.OrdinalIgnoreCase))
            {
                recordsToDelete = this.fileCabinetService.FindByLastName(value).ToList();
            }
            else if (columnName.Equals("dateOfBirth", StringComparison.OrdinalIgnoreCase))
            {
                recordsToDelete = this.fileCabinetService.FindByDateOfBirth(value).ToList();
            }
            else
            {
                Console.WriteLine("Unknown column name. Allowed names are: firstName, lastName and dateOfBirth");
                return;
            }

            if (recordsToDelete.Count == 0)
            {
                Console.WriteLine("No records that satisfy the condition.");
                return;
            }

            foreach (var record in recordsToDelete)
            {
                try
                {
                    this.fileCabinetService.RemoveRecord(record.Id);
                    Console.WriteLine($"Record {record.Id} is deleted.");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}