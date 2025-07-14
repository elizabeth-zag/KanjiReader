namespace KanjiReader.Infrastructure.Database.Models;

public class ProcessingResultDb
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
    public string Url { get; set; }
}