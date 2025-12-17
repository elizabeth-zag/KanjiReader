using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Database.Models;

public class Text
{
    public int Id { get; set; }
    public GenerationSourceType SourceType { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string Url { get; set; } 
    
    public Text() {}
    
    public Text(
        GenerationSourceType sourceType, 
        string title, 
        string text, 
        string url)
    {
        SourceType = sourceType;
        Title = title;
        Content = text;
        Url = url;
    }
}