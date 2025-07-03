namespace KanjiReader.Domain.TextProcessing.DomainObjects;

public class TextProcessingResult
{
    public bool IsSuccess;
    public string Url;
    public string Text;

    public TextProcessingResult(bool isSuccess, string url, string text)
    {
        IsSuccess = isSuccess;
        Url = url;
        Text = text;
    }

    public static TextProcessingResult CreateSuccessResult(string url, string text)
    {
        return new TextProcessingResult(true, url, text);
    }

    public static TextProcessingResult CreateFailResult()
    {
        return new TextProcessingResult(false, String.Empty, string.Empty);
    }
}