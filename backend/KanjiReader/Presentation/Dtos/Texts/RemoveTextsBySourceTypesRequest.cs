using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class RemoveTextsBySourceTypesRequest
{
    public GenerationSourceType[] SourceTypes { get; set; }
}