namespace KanjiReader.Infrastructure.Repositories.Cache;

public interface IWatanocCacheRepository
{
    Task SetArticleUrls(string category, int pageNumber, string[] articleUrls);
    Task<string[]> GetArticleUrls(string category, int pageNumber);
    Task SetHtml(string url, string html);
    Task<string> GetHtml(string url);
}