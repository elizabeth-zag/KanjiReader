namespace KanjiReader.Domain.DomainObjects.EventData;

public class SatoriParsingData
{
    public int SeriesNumber { get; init; }
    public int MaxSeriesNumber { get; set; }
    
    public SatoriParsingData UpdateMaxNumber(int newMax)
    {
        MaxSeriesNumber = newMax;
        return this;
    }
}