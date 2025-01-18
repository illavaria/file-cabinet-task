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
    /// Gets the number of records.
    /// </summary>
    /// <returns>Number of records.</returns>
    public int GetStat();

    /// <summary>
    /// Edits the record.
    /// </summary>
    /// <param name="id">Record Id.</param>
    /// <param name="parameters">Record's parameters.</param>
    /// <exception cref="ArgumentException">Thrown if record with such id doesn't exist.</exception>
    public void EditRecord(int id, FileCabinetRecordsParameters? parameters);

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
    public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string? firstName);

    /// <summary>
    /// Find records by last name.
    /// </summary>
    /// <param name="lastName">Last name to search.</param>
    /// <returns>List of records with the matched last name.</returns>
    public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string? lastName);

    /// <summary>
    /// Find records by date of birth.
    /// </summary>
    /// <param name="dateOfBirthString">String representing date of birth.</param>
    /// <returns>List of records with the matched date of birth.</returns>
    public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString);
}