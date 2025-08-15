using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Database.Models;

public class ProcessingResult
{
    public int Id { get; set; }
    public GenerationSourceType SourceType { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
    public string Title { get; set; }
    public string Url { get; set; } 
    public double UnknownKanjiRatio { get; set; }
    public char[] UnknownKanji { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    
    public ProcessingResult() {}
    
    public ProcessingResult(
        string userId, 
        GenerationSourceType sourceType, 
        string title, 
        string text, 
        string url, 
        double ratio,
        char[] unknownKanji,
        DateTime createDate)
    {
        UserId = userId;
        SourceType = sourceType;
        Title = title;
        Text = text;
        Url = url;
        UnknownKanjiRatio = ratio;
        UnknownKanji = unknownKanji;
        CreateDate = createDate;
    }
}