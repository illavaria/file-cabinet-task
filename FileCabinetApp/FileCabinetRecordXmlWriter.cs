using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp;

/// <summary>
/// Class for writing records to xml file.
/// </summary>
public class FileCabinetRecordXmlWriter
{
    private XmlWriter writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
    /// </summary>
    /// <param name="writer">XmlWriter used for writing.</param>
    public FileCabinetRecordXmlWriter(XmlWriter writer)
    {
        this.writer = writer;
    }

    /// <summary>
    /// Writes a record in xml format.
    /// </summary>
    /// <param name="record">A record to write.</param>
    /// <exception cref="ArgumentNullException">Thrown if record is null.</exception>
    public void Write(FileCabinetRecord record)
    {
        _ = record ?? throw new ArgumentNullException(nameof(record));
        var a = new XmlSerializer(typeof(FileCabinetRecordXml));
        var recordXml = new FileCabinetRecordXml()
        {
            Id = record.Id,
            Name = new Name() { FirstName = record.FirstName, LastName = record.LastName },
            DateOfBirthString = record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            NumberOfChildren = record.NumberOfChildren,
            YearIncome = record.YearIncome,
            Gender = record.Gender,
        };
        var namespaceXml = new XmlSerializerNamespaces();
        namespaceXml.Add(string.Empty, string.Empty);
        a.Serialize(this.writer, recordXml, namespaceXml);
    }
}