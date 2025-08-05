using KanjiReader.Infrastructure.Repositories;
using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Redis;

public class RedisKanjiCacheRepository(IConnectionMultiplexer redis) : IKanjiCacheRepository
{
    public async Task SetUserKanji(string userId, IReadOnlySet<char> kanji)
    {
        var value = new string(kanji.ToArray());
        
        var db = redis.GetDatabase();
        await db.StringSetAsync(GetKey(userId), value);
    }

    public async Task<IReadOnlySet<char>> GetUserKanji(string userId)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetAsync(GetKey(userId));
        
        return result.ToString().ToCharArray().ToHashSet();
    }
    
    private static string GetKey(string userId) => $"userid:{userId}";
}
