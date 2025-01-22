using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetApp;

/// <summary>
/// Class represents record for xml storing.
/// </summary>
[XmlRoot("record")]
public class FileCabinetRecordXml
{
    /// <summary>
    /// Gets or sets record's id.
    /// </summary>
    [XmlElement("id")]
    public int Id { get; set; }

    [XmlElement("name")]
    public Name Name { get; set; }

    [XmlElement("dateOfBirth")]
    public string DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets number of children.
    /// </summary>
    [XmlElement("numberOfChildren")]
    public short NumberOfChildren { get; set; }

    /// <summary>
    /// Gets or sets year income.
    /// </summary>
    [XmlElement("yearIncome")]
    public decimal YearIncome { get; set; }

    [XmlElement("gender")]
    public string Gender { get; set; }
}

/// <summary>
/// Class represents record's name.
/// </summary>
[XmlType("name")]
public class Name
{
    /// <summary>
    /// Gets or sets first name.
    /// </summary>
    [XmlAttribute("first")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets last name.
    /// </summary>
    [XmlAttribute("last")]
    public string? LastName { get; set; }
}