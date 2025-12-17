namespace KanjiReader.Infrastructure.Repositories.Cache;

public interface INhkCacheRepository
{
    Task SetArticleUrls(string[] articleUrls);
    Task<string[]> GetArticleUrls();
    Task SetHtml(string url, string html);
    Task<string> GetHtml(string url);
    Task SetHtmlTitle(string url, string title);
    Task<string> GetHtmlTitle(string url);
}