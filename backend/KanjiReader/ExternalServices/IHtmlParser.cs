namespace KanjiReader.ExternalServices;

public interface IHtmlParser
{
    Task<string> ParseHtml(string url, CancellationToken cancellationToken);
}