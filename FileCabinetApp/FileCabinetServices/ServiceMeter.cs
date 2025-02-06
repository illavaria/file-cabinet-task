using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp.FileCabinetServices;

/// <summary>
/// Class represents file cabinet service this timer ability.
/// </summary>
/// <param name="service">File cabinet service.</param>
/// <param name="writer">Text writer used to write log.</param>
public class ServiceMeter(IFileCabinetService service, TextWriter writer) : IFileCabinetService
{
    private readonly IFileCabinetService service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly TextWriter writer = writer ?? throw new ArgumentNullException(nameof(writer));

    /// <inheritdoc/>
    public int CreateRecord(FileCabinetRecordsParameters? parameters) =>
        this.MeasureExecutionTime(nameof(this.CreateRecord), () => this.service.CreateRecord(parameters));

    /// <inheritdoc/>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords() =>
        this.MeasureExecutionTime(nameof(this.GetRecords), this.service.GetRecords);

    /// <inheritdoc/>
    public int GetNumberOfAllRecords() =>
        this.MeasureExecutionTime(nameof(this.GetNumberOfAllRecords), this.service.GetNumberOfAllRecords);

    /// <inheritdoc/>
    public int GetNumberOfDeletedRecords() =>
        this.MeasureExecutionTime(nameof(this.GetNumberOfDeletedRecords), this.service.GetNumberOfDeletedRecords);

    /// <inheritdoc/>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters) =>
        this.MeasureExecutionTime(nameof(this.EditRecord), () => this.service.EditRecord(id, parameters));

    /// <inheritdoc/>
    public void RemoveRecord(int id) =>
        this.MeasureExecutionTime(nameof(this.RemoveRecord), () => this.service.RemoveRecord(id));

    /// <inheritdoc/>
    public FileCabinetRecord? FindById(int id) =>
        this.MeasureExecutionTime(nameof(this.FindById), () => this.service.FindById(id));

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByFirstName(string? firstName) =>
        this.MeasureExecutionTime(nameof(this.FindByFirstName), () => this.service.FindByFirstName(firstName));

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByLastName(string? lastName) =>
        this.MeasureExecutionTime(nameof(this.FindByLastName), () => this.service.FindByLastName(lastName));

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString) =>
        this.MeasureExecutionTime(nameof(this.FindByDateOfBirth), () => this.service.FindByDateOfBirth(dateOfBirthString));

    /// <inheritdoc/>
    public IEnumerable<FileCabinetRecord> Find(Dictionary<string, string> conditions) =>
        this.MeasureExecutionTime(nameof(this.Find), () => this.service.Find(conditions));

    /// <inheritdoc/>
    public FileCabinetServiceSnapshot MakeSnapshot() =>
        this.MeasureExecutionTime(nameof(this.MakeSnapshot), this.service.MakeSnapshot);

    /// <inheritdoc/>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref Collection<string> errorsList)
    {
        var stopwatch = Stopwatch.StartNew();
        this.service.Restore(snapshot, ref errorsList);
        stopwatch.Stop();
        this.writer.WriteLine($"{nameof(this.Restore)} method execution duration is {stopwatch.ElapsedTicks} ticks or {stopwatch.ElapsedMilliseconds} milliseconds.");
    }

    /// <inheritdoc/>
    public void Purge() =>
        this.MeasureExecutionTime(nameof(this.Purge), this.service.Purge);

    /// <inheritdoc/>
    public void InsertRecord(int id, FileCabinetRecordsParameters parameters) =>
        this.MeasureExecutionTime(nameof(this.InsertRecord), () => this.service.InsertRecord(id, parameters));

    private T MeasureExecutionTime<T>(string methodName, Func<T> func)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = func.Invoke();
        stopwatch.Stop();
        this.writer.WriteLine($"{methodName} method execution duration is {stopwatch.ElapsedTicks} ticks or {stopwatch.ElapsedMilliseconds} milliseconds.");
        return result;
    }

    private void MeasureExecutionTime(string methodName, Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action.Invoke();
        stopwatch.Stop();
        this.writer.WriteLine($"{methodName} method execution duration is {stopwatch.ElapsedTicks} ticks or {stopwatch.ElapsedMilliseconds} milliseconds.");
    }
}