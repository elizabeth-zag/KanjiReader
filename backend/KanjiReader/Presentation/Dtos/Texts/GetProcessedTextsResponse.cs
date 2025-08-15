namespace KanjiReader.Presentation.Dtos.Texts;

public class GetProcessedTextsResponse
{
    public ProcessingResultDto[] ProcessedTexts { get; set; } = [];
    public int AllTextsCount { get; set; }
}