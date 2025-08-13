using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class ProcessingResultDto
{
    public GenerationSourceType SourceType { get; set; }
    public string Text { get; set; }
    public string Url { get; set; }
}