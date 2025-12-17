namespace KanjiReader.Domain.DomainObjects.TextProcessingData;

public class NhkParsingData(int lastId)
{
    public int LastId { get; private set; } = lastId;
}