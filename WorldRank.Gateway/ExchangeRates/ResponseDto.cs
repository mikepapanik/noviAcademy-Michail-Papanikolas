using System.Xml.Serialization;

namespace WorldRank.Gateway.ExchangeRates;

[XmlRoot(
    ElementName = "Envelope",
    Namespace = "http://www.gesmes.org/xml/2002-08-01")]
public sealed class ResponseDto
{
    [XmlElement(
        ElementName = "Cube",
        Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public CubeContainerDto Cube { get; set; } = new();
}

public sealed class CubeContainerDto
{
    [XmlElement(
        ElementName = "Cube",
        Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public RatesCubeDto Rates { get; set; } = new();
}

public sealed class RatesCubeDto
{
    [XmlAttribute(
        AttributeName = "time",
        DataType = "date")]
    public DateTime Date { get; set; }

    [XmlElement(
        ElementName = "Cube",
        Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public List<RateCubeDto> Rates { get; set; } = [];
}

public sealed class RateCubeDto
{
    [XmlAttribute(AttributeName = "currency")]
    public string Currency { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "rate")]
    public decimal Rate { get; set; }
}