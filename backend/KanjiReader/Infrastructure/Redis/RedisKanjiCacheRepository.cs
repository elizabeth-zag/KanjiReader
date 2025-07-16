using KanjiReader.Infrastructure.Repositories;
using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Redis;

public class RedisKanjiCacheRepository : IKanjiCacheRepository
{
    private readonly IConnectionMultiplexer _redis;

    public RedisKanjiCacheRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetUserKanji(string userId, IReadOnlySet<char> kanji)
    {
        var key = $"userid:{userId}";
        var value = new string(kanji.ToArray());
        
        var db = _redis.GetDatabase();
        await db.StringSetAsync(key, value);
    }

    public async Task<IReadOnlySet<char>> GetUserKanji(string userId)
    {
        var db = _redis.GetDatabase();
        var result = await db.StringGetAsync(userId);
        
        return result.ToString().ToCharArray().ToHashSet();
    }
}
