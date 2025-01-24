using System.Collections.ObjectModel;
using System.Xml;

namespace FileCabinetApp;

/// <summary>
/// Class represents a snapshot of file cabinet service.
/// </summary>
public class FileCabinetServiceSnapshot
{
    private FileCabinetRecord[] records;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
    /// </summary>
    public FileCabinetServiceSnapshot()
    {
        this.records = Array.Empty<FileCabinetRecord>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
    /// </summary>
    /// <param name="records">Array of records of file cabinet service.</param>
    public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
    {
        this.records = records;
    }

    /// <summary>
    /// Gets records as read only collection.
    /// </summary>
    public ReadOnlyCollection<FileCabinetRecord> Records => this.records.AsReadOnly();

    /// <summary>
    /// Saves file cabinet state (records copy) in csv format.
    /// </summary>
    /// <param name="streamWriter">StreamWriter used for saving.</param>
    public void SaveToCsv(StreamWriter streamWriter)
    {
        var recordCsvWriter = new FileCabinetRecordCsvWriter(streamWriter);
        foreach (var record in this.records)
        {
            recordCsvWriter.Write(record);
        }
    }

    /// <summary>
    /// Saves file cabinet state (records copy) in xml format.
    /// </summary>
    /// <param name="streamWriter">StreamWriter used for saving.</param>
    public void SaveToXml(StreamWriter streamWriter)
    {
        using var xmlWriter = new XmlTextWriter(streamWriter);
        var recordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("records");
        foreach (var record in this.records)
        {
            recordXmlWriter.Write(record);
        }

        xmlWriter.WriteEndElement();
    }

    /// <summary>
    /// Loads records from csv file.
    /// </summary>
    /// <param name="streamReader">Stream reader used for reading csv file.</param>
    public void LoadFromCsv(StreamReader streamReader)
    {
        var recordCsvReader = new FileCabinetRecordCsvReader(streamReader);
        this.records = recordCsvReader.ReadAll().ToArray();
    }

    /// <summary>
    /// Loads records from xml file.
    /// </summary>
    /// <param name="streamReader">Stream reader used for reading xml file.</param>
    public void LoadFromXml(StreamReader streamReader)
    {
        using var xmlReader = XmlReader.Create(streamReader, new XmlReaderSettings { IgnoreWhitespace = true });
        var recordXmlReader = new FileCabinetRecordXmlReader(xmlReader);
        this.records = recordXmlReader.ReadAll().ToArray();
    }
}