using System.Collections.ObjectModel;

namespace FileCabinetApp;

/// <summary>
/// Interface of file cabinet service.
/// </summary>
public interface IFileCabinetService
{
    /// <summary>
    /// Creates a new record.
    /// </summary>
    /// <param name="parameters">Record's parameters.</param>
    /// <returns>Id of the created record.</returns>
    public int CreateRecord(FileCabinetRecordsParameters? parameters);

    /// <summary>
    /// Returns all records.
    /// </summary>
    /// <returns>Records list.</returns>
    public ReadOnlyCollection<FileCabinetRecord> GetRecords();

    /// <summary>
    /// Gets the number of all records.
    /// </summary>
    /// <returns>Number of all records.</returns>
    public int GetNumberOfAllRecords();

    /// <summary>
    /// Gets the number of deleted records.
    /// </summary>
    /// <returns>Number of deleted records.</returns>
    public int GetNumberOfDeletedRecords();

    /// <summary>
    /// Edits the record.
    /// </summary>
    /// <param name="id">Record Id.</param>
    /// <param name="parameters">Record's parameters.</param>
    /// <exception cref="ArgumentException">Thrown if record with such id doesn't exist.</exception>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters);

    /// <summary>
    /// Removes the record.
    /// </summary>
    /// <param name="id">Record Id.</param>
    /// <exception cref="ArgumentException">Thrown if record with such id doesn't exist.</exception>
    public void RemoveRecord(int id);

    /// <summary>
    /// Finds record by its id.
    /// </summary>
    /// <param name="id">Record Id.</param>
    /// <returns>Record if found, otherwise null.</returns>
    public FileCabinetRecord? FindById(int id);

    /// <summary>
    /// Find records by first name.
    /// </summary>
    /// <param name="firstName">First name to search.</param>
    /// <returns>List of records with the matched first name.</returns>
    public IEnumerable<FileCabinetRecord> FindByFirstName(string? firstName);

    /// <summary>
    /// Find records by last name.
    /// </summary>
    /// <param name="lastName">Last name to search.</param>
    /// <returns>List of records with the matched last name.</returns>
    public IEnumerable<FileCabinetRecord> FindByLastName(string? lastName);

    /// <summary>
    /// Find records by date of birth.
    /// </summary>
    /// <param name="dateOfBirthString">String representing date of birth.</param>
    /// <returns>List of records with the matched date of birth.</returns>
    public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString);

    /// <summary>
    /// Makes snapshot of the current file cabinet state.
    /// </summary>
    /// <returns>A new snapshot.</returns>
    public FileCabinetServiceSnapshot MakeSnapshot();

    /// <summary>
    /// Restores file cabinet state to the snapshot's state.
    /// </summary>
    /// <param name="snapshot">Snapshot representing state for setting.</param>
    /// <param name="errorsList">List of errors that occured during restoring.</param>
    public void Restore(FileCabinetServiceSnapshot snapshot, ref List<string> errorsList);

    /// <summary>
    /// Removes deleted records from the list.
    /// </summary>
    public void Purge();
}