using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.FileCabinetServices;

/// <summary>
/// Class represents logger decorator for file cabinet service.
/// </summary>
/// <param name="service">File cabinet service that is decorated.</param>
/// <param name="writer">Text writer used to write log.</param>
public class ServiceLogger(IFileCabinetService service, TextWriter writer) : IFileCabinetService
{
    private readonly IFileCabinetService service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly TextWriter writer = writer ?? throw new ArgumentNullException(nameof(writer));

    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        this.WriteLog($"Calling CreateRecord() with {FormatParameters(parameters)}");
        var result = this.service.CreateRecord(parameters);
        this.WriteLog($"CreateRecord() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        this.WriteLog("Calling GetRecords()");
        var result = this.service.GetRecords();
        this.WriteLog($"GetRecords() returned '{result.Count} records'");
        return result;
    }

    /// <inheritdoc/>
    public int GetNumberOfAllRecords()
    {
        this.WriteLog("Calling GetNumberOfAllRecords()");
        var result = this.service.GetNumberOfAllRecords();
        this.WriteLog($"GetNumberOfAllRecords() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords()
    {
        this.WriteLog("Calling GetNumberOfDeletedRecords()");
        var result = this.service.GetNumberOfDeletedRecords();
        this.WriteLog($"GetNumberOfDeletedRecords() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        this.WriteLog($"Calling EditRecord() with Id = '{id}', {FormatParameters(parameters)}");
        this.service.EditRecord(id, parameters);
        this.WriteLog("EditRecord() completed");
    }

    /// <inheritdoc/>
    public void RemoveRecord(int id)
    {
        this.WriteLog($"Calling RemoveRecord() with Id = '{id}'");
        this.service.RemoveRecord(id);
        this.WriteLog("RemoveRecord() completed");
    }

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id)
    {
        this.WriteLog($"Calling FindById() with Id = '{id}'");
        var result = this.service.FindById(id);
        this.WriteLog($"FindById() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByFirstName(string? firstName)
    {
        this.WriteLog($"Calling FindByFirstName() with firstName = '{firstName}'");
        var result = this.service.FindByFirstName(firstName);
        this.WriteLog($"FindByFirstName() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByLastName(string? lastName)
    {
        this.WriteLog($"Calling FindByLastName() with lastName = '{lastName}'");
        var result = this.service.FindByFirstName(lastName);
        this.WriteLog($"FindByLastName() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
    {
        this.WriteLog($"Calling FindByDateOfBirth() with dateOfBirth = '{dateOfBirthString}'");
        var result = this.service.FindByDateOfBirth(dateOfBirthString);
        this.WriteLog($"FindByDateOfBirth() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> Find(Dictionary<string, string> conditions)
    {
        this.WriteLog($"Calling Find() with {string.Join(", ", conditions.Select(field => $"{field.Key} = {field.Value}"))}");
        var result = this.service.Find(conditions);
        this.WriteLog($"Find() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot()
    {
        this.WriteLog($"Calling MakeSnapshot()");
        var result = this.service.MakeSnapshot();
        this.WriteLog($"MakeSnapshot() completed");
        return result;
    }

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref Collection<string> errorsList)
    {
        this.WriteLog("Calling Restore()");
        this.service.Restore(snapshot, ref errorsList);
        if (errorsList.Count != 0)
        {
            this.WriteLog($"Restore() completed with the following errors: ");
            foreach (var error in errorsList)
            {
               this.writer.WriteLine(error + Environment.NewLine);
            }
        }
        else
        {
            this.WriteLog("Restore() completed without errors.");
        }
    }

    /// <inheritdoc/>
    public void Purge()
    {
        this.WriteLog("Calling Purge()");
        this.service.Purge();
        this.WriteLog("Purge() completed");
    }

    /// <inheritdoc/>
    public void InsertRecord(int id, FileCabinetRecordsParameters parameters)
    {
        this.WriteLog($"Calling Insert() with Id = '{id}', {FormatParameters(parameters)}");
        this.service.InsertRecord(id, parameters);
        this.WriteLog("Insert() completed");
    }

    private static string FormatParameters(FileCabinetRecordsParameters? parameters)
        => parameters is null
            ? "null"
            : $"FirstName = '{parameters.FirstName}', LastName = '{parameters.LastName}', " +
              $"DateOfBirth = '{parameters.DateOfBirth:MM/dd/yyyy}', NumberOfChildren = '{parameters.NumberOfChildren}', " +
              $"YearIncome = '{parameters.YearIncome}', Gender = '{parameters.Gender}'";

    private void WriteLog(string message)
    {
        var logMessage = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - {message}";
        this.writer.WriteLine(logMessage);
    }
}