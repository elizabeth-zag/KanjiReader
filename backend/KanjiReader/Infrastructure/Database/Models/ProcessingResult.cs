using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Database.Models;

public class ProcessingResult
{
    public int Id { get; set; }
    public GenerationSourceType SourceType { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
    public string Url { get; set; } 
    public double UnknownKanjiRatio { get; set; }
    public char[] UnknownKanji { get; set; } // todo: think what to do with this field
    
    public ProcessingResult() {}
    
    public ProcessingResult(
        string userId, 
        GenerationSourceType sourceType, 
        string text, 
        string url, 
        double ratio,
        char[] unknownKanji)
    {
        UserId = userId;
        SourceType = sourceType;
        Text = text;
        Url = url;
        UnknownKanjiRatio = ratio;
        UnknownKanji = unknownKanji;
    }
}