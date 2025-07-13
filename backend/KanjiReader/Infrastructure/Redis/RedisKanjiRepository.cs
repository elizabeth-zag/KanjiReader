using KanjiReader.Infrastructure.Repositories;
using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Redis;

public class RedisKanjiRepository : IKanjiRepository
{
    private readonly IConnectionMultiplexer _redis;

    public RedisKanjiRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetUserKanji(string userId, char[] kanji)
    {
        var key = $"userid:{userId}";
        var value = new string(kanji);
        
        var db = _redis.GetDatabase();
        await db.StringSetAsync(key, value);
    }

    public async Task<char[]> GetUserKanji(string userId)
    {
        var db = _redis.GetDatabase();
        var result = await db.StringGetAsync(userId);
        
        return result.ToString().ToCharArray();
    }
}
