using System.Xml;

namespace FileCabinetApp;

/// <summary>
/// Class represents a snapshot of file cabinet service.
/// </summary>
public class FileCabinetServiceSnapshot
{
    private readonly FileCabinetRecord[] records;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
    /// </summary>
    /// <param name="records">Array of records of file cabinet service.</param>
    public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
    {
        this.records = records;
    }

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
}