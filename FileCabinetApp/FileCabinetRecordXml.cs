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
    public string DateOfBirthString { get; set; }

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
    public string GenderString
    {
        get => Gender.ToString(CultureInfo.InvariantCulture);
        set => Gender = string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(value) : value[0];
    }

    /// <summary>
    /// Gets or sets gender.
    /// </summary>
    [XmlIgnore]
    public char Gender { get; set; }
}

[XmlType("name")]
public class Name
{
    [XmlAttribute("first")]
    public string? FirstName { get; set; }

    [XmlAttribute("last")]
    public string? LastName { get; set; }
}