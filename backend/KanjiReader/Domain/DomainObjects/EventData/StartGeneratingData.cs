namespace KanjiReader.Domain.DomainObjects.EventData;

public class StartGeneratingData
{
    public GenerationSourceType[] SourceTypes { get; init; }
}