namespace KanjiReader.Domain.DomainObjects.EventData;

public class NhkParsingData
{
    public DateTime FirstDate { get; init; }
    public DateTime LastDate { get; init; }
    public DateTime CurrentDate { get; init; }
    public DateTime[] OrderedDates { get; set; }
    
    public NhkParsingData SetOrderedDates(DateTime[] dates)
    {
        OrderedDates = dates.OrderBy(d => d).ToArray();
        return this;
    }
}