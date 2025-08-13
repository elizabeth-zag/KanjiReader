using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class StartCollectingRequest
{
    public GenerationSourceType[] Sources { get; set; }
}