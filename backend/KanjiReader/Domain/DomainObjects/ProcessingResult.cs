namespace KanjiReader.Domain.DomainObjects;

public class ProcessingResult
{
    public string UserId;
    public string Text;
    public string Url;
    public bool IsRemoved;

    public ProcessingResult(string userId, string text, string url, bool isRemoved)
    {
        UserId = userId;
        Text = text;
        Url = url;
        IsRemoved = isRemoved;
    }
}