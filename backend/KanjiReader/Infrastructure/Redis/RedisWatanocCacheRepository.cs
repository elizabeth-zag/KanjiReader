using KanjiReader.Infrastructure.Repositories.Cache;
using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Redis;

public class RedisWatanocCacheRepository(IConnectionMultiplexer redis) : IWatanocCacheRepository
{
    private const string Separator = ",";
    private static string GetArticlesKey(string category, int pageNumber) => $"watanoc-article-urls:{category}:{pageNumber}";
    private static string GetHtmlTitleKey(string url) => $"watanoc-html-title:{url}";
    private static string GetHtmlKey(string url) => $"watanoc-html:{url}";
    
    public async Task SetArticleUrls(string category, int pageNumber, string[] articleUrls)
    {
        var value = string.Join(Separator, articleUrls);
        
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetArticlesKey(category, pageNumber), value, TimeSpan.FromDays(1)); // todo: config
    }
    
    public async Task<string[]> GetArticleUrls(string category, int pageNumber)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetArticlesKey(category, pageNumber));
        
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