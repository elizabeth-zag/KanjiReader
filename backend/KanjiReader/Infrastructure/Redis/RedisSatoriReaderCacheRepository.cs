using KanjiReader.Infrastructure.Repositories.Cache;
using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Redis;

public class RedisSatoriReaderCacheRepository(IConnectionMultiplexer redis) : ISatoriReaderCacheRepository
{
    private const string Separator = ",";
    private static string GetSeriesKey() => "satori-series-urls";
    private static string GetArticlesKey(string seriesUrl) => $"satori-article-urls:{seriesUrl}";
    private static string GetHtmlTitleKey(string url) => $"satori-html-title:{url}";
    private static string GetHtmlKey(string url) => $"satori-html:{url}";
    
    public async Task SetSeriesUrls(string[] urls)
    {
        var value = string.Join(Separator, urls);
        
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetSeriesKey(), value, TimeSpan.FromDays(1)); // todo: config
    }
    
    public async Task<string[]> GetSeriesUrls()
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetSeriesKey());
        
        return result.IsNullOrEmpty ? [] : result.ToString().Split(Separator);
    }
    
    public async Task SetArticleUrls(string seriesUrl, string[] articleUrls)
    {
        var value = string.Join(Separator, articleUrls);
        
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetArticlesKey(seriesUrl), value, TimeSpan.FromDays(1)); // todo: config
    }
    
    public async Task<string[]> GetArticleUrls(string seriesUrl)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetArticlesKey(seriesUrl));
        
        return result.IsNullOrEmpty ? [] : result.ToString().Split(Separator);
    }
    
    public async Task SetHtml(string url, string html)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetHtmlKey(url), html, TimeSpan.FromDays(1)); // todo: config
    }
    
    public async Task<string> GetHtml(string url)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetHtmlKey(url));
        
        return result.ToString();
    }
    
    public async Task SetHtmlTitle(string url, string title)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetHtmlTitleKey(url), title, TimeSpan.FromDays(1)); // todo: config
    }
    
    public async Task<string> GetHtmlTitle(string url)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetHtmlTitleKey(url));
        
        return result.ToString();
    }
}