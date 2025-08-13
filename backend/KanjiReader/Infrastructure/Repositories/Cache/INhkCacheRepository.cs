namespace KanjiReader.Infrastructure.Repositories.Cache;

public interface INhkCacheRepository
{
    Task SetArticleUrls(Dictionary<DateTime, string[]> articleUrls);
    Task<Dictionary<DateTime, string[]>> GetArticleUrls();
    Task SetHtml(string url, string html);
    Task<string> GetHtml(string url);
}