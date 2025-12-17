namespace KanjiReader.Domain.DomainObjects.TextProcessingData;

public class SatoriParsingData(int seriesNumber)
{
    public int SeriesNumber { get; private set; } = seriesNumber;
}