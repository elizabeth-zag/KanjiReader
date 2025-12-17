namespace KanjiReader.Domain.DomainObjects.TextProcessingData;

public class WatanocParsingData(string category, int pageNumber)
{
    public string Category { get; private set; } = category;
    public int PageNumber { get; private set; } = pageNumber;
}