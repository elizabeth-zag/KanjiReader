using System.Text.Json;
using KanjiReader.Domain.Common.Options.CacheOptions;
using KanjiReader.Infrastructure.Repositories.Cache;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Redis;

public class RedisNhkCacheRepository(IConnectionMultiplexer redis, IOptionsMonitor<NhkCacheOptions> options) : INhkCacheRepository
{
    private static string GetArticlesKey() => "nhk-article-urls";
    private static string GetHtmlTitleKey(string url) => $"nhk-html-title:{url}";
    private static string GetHtmlKey(string url) => $"nhk-html:{url}";
    
    public async Task SetArticleUrls(Dictionary<DateTime, string[]> articleUrls)
    {
        var value = JsonSerializer.Serialize(articleUrls);
        
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetArticlesKey(), value, TimeSpan.FromDays(options.CurrentValue.TtlDays));
    }
    
    public async Task<Dictionary<DateTime, string[]>> GetArticleUrls()
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetArticlesKey());
        
        return !result.IsNullOrEmpty
            ? JsonSerializer.Deserialize<Dictionary<DateTime, string[]>>(result!)!
            : new Dictionary<DateTime, string[]>();
    }
    
    public async Task SetHtml(string url, string html)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetHtmlKey(url), html, TimeSpan.FromDays(options.CurrentValue.TtlDays));
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
        await db.StringSetAsync(GetHtmlTitleKey(url), title, TimeSpan.FromDays(options.CurrentValue.TtlDays));
    }
    
    public async Task<string> GetHtmlTitle(string url)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetHtmlTitleKey(url));
        
        return result.ToString();
    }
}