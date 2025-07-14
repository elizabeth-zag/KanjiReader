namespace KanjiReader.Domain.DomainObjects;

public class ProcessingResult
{
    public string UserId;
    public string Text;
    public string Url;

    public ProcessingResult(string userId, string text, string url)
    {
        UserId = userId;
        Text = text;
        Url = url;
    }
}