namespace KanjiReader.Domain.DomainObjects.EventData;

public class NhkParsingData
{
    public DateTime FirstDate { get; init; }
    public DateTime LastDate { get; init; }
    public DateTime CurrentDate { get; init; }
}