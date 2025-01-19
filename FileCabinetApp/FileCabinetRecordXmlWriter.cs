using System.Globalization;
using System.Xml;

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
        this.writer.WriteStartElement("record");
        this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));

        this.writer.WriteStartElement("name");
        this.writer.WriteAttributeString("first", record.FirstName);
        this.writer.WriteAttributeString("last", record.LastName);
        this.writer.WriteEndElement();

        this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

        this.writer.WriteElementString("numberOfChildren", record.NumberOfChildren.ToString(CultureInfo.InvariantCulture));
        this.writer.WriteElementString("yearIncome", record.YearIncome.ToString(CultureInfo.InvariantCulture));
        this.writer.WriteElementString("gender", record.Gender.ToString());

        this.writer.WriteEndElement();
    }
}