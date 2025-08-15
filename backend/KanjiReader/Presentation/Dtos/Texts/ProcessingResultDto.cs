using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class ProcessingResultDto
{
    public GenerationSourceType SourceType { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Url { get; set; }
    public double Ratio { get; set; }
    public char[] UnknownKanji { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}