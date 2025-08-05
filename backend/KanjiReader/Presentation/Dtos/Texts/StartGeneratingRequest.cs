using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class StartGeneratingRequest
{
    public GenerationSourceType[] SourceTypes { get; set; }
}