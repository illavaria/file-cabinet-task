using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp;

/// <summary>
/// Class for writing records to xml file.
/// </summary>
public class FileCabinetRecordXmlWriter(XmlWriter writer)
{
    private XmlWriter writer = writer ?? throw new ArgumentNullException(nameof(writer));

    /// <summary>
    /// Writes a record in xml format.
    /// </summary>
    /// <param name="record">A record to write.</param>
    /// <exception cref="ArgumentNullException">Thrown if record is null.</exception>
    public void Write(FileCabinetRecord record)
    {
        _ = record ?? throw new ArgumentNullException(nameof(record));
        var serializer = new XmlSerializer(typeof(FileCabinetRecordXml));
        var recordXml = FileCabinetRecordMapper.MapToXml(record);
        var namespaceXml = new XmlSerializerNamespaces();
        namespaceXml.Add(string.Empty, string.Empty);
        serializer.Serialize(this.writer, recordXml, namespaceXml);
    }
}