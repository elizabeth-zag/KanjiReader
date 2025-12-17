namespace KanjiReader.Domain.Common.Options;

public class TextProcessingOptions
{
    public int TextCountLimit { get; set; }
    public int TextProcessingCount { get; set; }
    public int CooldownHours { get; set; }
    public int AiGenerationCooldownMinutes { get; set; }
    public int AiGenerationTokenLimit { get; set; }
    public int MinKanji { get; set; }
}