namespace KanjiReader.Infrastructure.Repositories.Cache;

public interface ISatoriReaderCacheRepository
{
    Task SetSeriesUrls(string[] urls);
    Task<string[]> GetSeriesUrls();
    Task SetArticleUrls(string seriesUrl, string[] articleUrls);
    Task<string[]> GetArticleUrls(string seriesUrl);
    Task SetHtml(string url, string html);
    Task<string> GetHtml(string url);
    Task SetHtmlTitle(string url, string title);
    Task<string> GetHtmlTitle(string url);
}