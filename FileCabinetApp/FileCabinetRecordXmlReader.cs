using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp;

/// <summary>
/// Class represents xml reader for file cabinet records.
/// </summary>
/// <param name="reader">Xml reader used to read.</param>
public class FileCabinetRecordXmlReader(XmlReader reader)
{
    private XmlReader reader = reader;

    /// <summary>
    /// Reads all records from reader.
    /// </summary>
    /// <returns>List of records.</returns>
    public IList<FileCabinetRecord> ReadAll()
    {
        var records = new List<FileCabinetRecord>();
        var serializer = new XmlSerializer(typeof(FileCabinetRecordXml));
        while (this.reader.Read())
        {
            if (this.reader is { NodeType: XmlNodeType.Element, Name: "record" })
            {
                using var subReader = this.reader.ReadSubtree();
                var xmlRecord = serializer.Deserialize(subReader) as FileCabinetRecordXml;
                records.Add(FileCabinetRecordMapper.MapFromXml(xmlRecord));
            }
        }

        return records;
    }
}