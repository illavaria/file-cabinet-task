using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp;

/// <summary>
/// Class represents file cabinet service this timer ability.
/// </summary>
/// <param name="service">File cabinet service.</param>
public class ServiceMeter(IFileCabinetService service) : IFileCabinetService
{
    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters) =>
        MeasureExecutionTime(nameof(this.CreateRecord), () => service.CreateRecord(parameters));

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords() =>
        MeasureExecutionTime(nameof(this.GetRecords), service.GetRecords);

    /// <inheritdoc/>
    public int GetNumberOfAllRecords() =>
        MeasureExecutionTime(nameof(this.GetNumberOfAllRecords), service.GetNumberOfAllRecords);

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords() =>
        MeasureExecutionTime(nameof(this.GetNumberOfDeletedRecords), service.GetNumberOfDeletedRecords);

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters) =>
        MeasureExecutionTime(nameof(this.EditRecord), () => service.EditRecord(id, parameters));

    /// <inheritdoc/>
    public void RemoveRecord(int id) =>
        MeasureExecutionTime(nameof(this.RemoveRecord), () => service.RemoveRecord(id));

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id) =>
        MeasureExecutionTime(nameof(this.CreateRecord), () => service.FindById(id));

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string? firstName) =>
        MeasureExecutionTime(nameof(this.CreateRecord), () => service.FindByFirstName(firstName));

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string? lastName) =>
        MeasureExecutionTime(nameof(this.CreateRecord), () => service.FindByLastName(lastName));

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString) =>
        MeasureExecutionTime(nameof(this.CreateRecord), () => service.FindByDateOfBirth(dateOfBirthString));

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot() =>
        MeasureExecutionTime(nameof(this.MakeSnapshot), service.MakeSnapshot);

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref List<string> errorsList)
    {
        var stopwatch = Stopwatch.StartNew();
        service.Restore(snapshot, ref errorsList);
        stopwatch.Stop();
        Console.WriteLine($"{nameof(this.Restore)} method execution duration is {stopwatch.ElapsedTicks} ticks.");
    }

    /// <inheritdoc/>
    public void Purge() =>
        MeasureExecutionTime(nameof(this.Purge), service.Purge);

    private static T MeasureExecutionTime<T>(string methodName, Func<T> func)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = func.Invoke();
        stopwatch.Stop();
        Console.WriteLine($"{methodName} method execution duration is {stopwatch.ElapsedTicks} ticks.");
        return result;
    }

    private static void MeasureExecutionTime(string methodName, Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine($"Method {methodName} started.");
        action.Invoke();
        stopwatch.Stop();
        Console.WriteLine($"{methodName} method execution duration is {stopwatch.ElapsedTicks} ticks.");
    }
}