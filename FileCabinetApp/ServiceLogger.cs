using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp;

public class ServiceLogger(IFileCabinetService service, string logFilePath) : IFileCabinetService
{
    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters)
    {
        this.WriteLog($"Calling CreateRecord() with {FormatParameters(parameters)}");
        var result = service.CreateRecord(parameters);
        this.WriteLog($"CreateRecord() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords()
    {
        this.WriteLog("Calling GetRecords()");
        var result = service.GetRecords();
        this.WriteLog($"GetRecords() returned '{result.Count} records'");
        return result;
    }

    /// <inheritdoc/>
    public int GetNumberOfAllRecords()
    {
        this.WriteLog("Calling GetNumberOfAllRecords()");
        var result = service.GetNumberOfAllRecords();
        this.WriteLog($"GetNumberOfAllRecords() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords()
    {
        this.WriteLog("Calling GetNumberOfDeletedRecords()");
        var result = service.GetNumberOfDeletedRecords();
        this.WriteLog($"GetNumberOfDeletedRecords() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters)
    {
        this.WriteLog($"Calling EditRecord() with Id = '{id}', {FormatParameters(parameters)}");
        service.EditRecord(id, parameters);
        this.WriteLog("EditRecord() completed");
    }

    /// <inheritdoc/>
    public void RemoveRecord(int id)
    {
        this.WriteLog($"Calling RemoveRecord() with Id = '{id}'");
        service.RemoveRecord(id);
        this.WriteLog("RemoveRecord() completed");
    }

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id)
    {
        this.WriteLog($"Calling FindById() with Id = '{id}'");
        var result = service.FindById(id);
        this.WriteLog($"FindById() returned '{result}'");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByFirstName(string? firstName)
    {
        this.WriteLog($"Calling FindByFirstName() with firstName = '{firstName}'");
        var result = service.FindByFirstName(firstName);
        this.WriteLog($"FindByFirstName() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByLastName(string? lastName)
    {
        this.WriteLog($"Calling FindByLastName() with lastName = '{lastName}'");
        var result = service.FindByFirstName(lastName);
        this.WriteLog($"FindByLastName() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
    {
        this.WriteLog($"Calling FindByDateOfBirth() with dateOfBirth = '{dateOfBirthString}'");
        var result = service.FindByDateOfBirth(dateOfBirthString);
        this.WriteLog($"FindByDateOfBirth() returned '{result.Count()}' records");
        return result;
    }

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot()
    {
        this.WriteLog($"Calling MakeSnapshot()");
        var result = service.MakeSnapshot();
        this.WriteLog($"MakeSnapshot() completed");
        return result;
    }

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref List<string> errorsList)
    {
        this.WriteLog("Calling Restore()");
        service.Restore(snapshot, ref errorsList);
        if (errorsList.Count != 0)
        {
            this.WriteLog($"Restore() completed with the following errors: ");
            foreach (var error in errorsList)
            {
                File.AppendAllText(logFilePath, error + Environment.NewLine);
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
        service.Purge();
        this.WriteLog("Purge() completed");
    }

    public void InsertRecord(int id, FileCabinetRecordsParameters parameters)
    {
        this.WriteLog($"Calling Insert() with Id = '{id}', {FormatParameters(parameters)}");
        service.InsertRecord(id, parameters);
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
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}