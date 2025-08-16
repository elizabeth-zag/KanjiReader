using StackExchange.Redis;
using System.Globalization;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Infrastructure.Repositories.Cache;

namespace KanjiReader.Infrastructure.Redis;

public class RedisKanjiCacheRepository(IConnectionMultiplexer redis, IKanjiRepository kanjiRepository) : IKanjiCacheRepository
{
    static readonly string KanjiPrefix = "kanji:";
    static string Cp(char c) => $"U+{(int)c:X4}";
    static string UserCharsKey(string userId) => $"user:{userId}:kanji:chars";
    static string KanjiKey(char c) => $"{KanjiPrefix}{Cp(c)}";
    static bool TryParseCp(string uplus)
    {
        if (string.IsNullOrEmpty(uplus) || uplus.Length < 3 || uplus[0] != 'U' || uplus[1] != '+') return false;
        return int.TryParse(uplus.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
    }
    static char CpToChar(string uplus)
    {
        var cp = int.Parse(uplus.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return (char)cp;
    }
    static HashEntry[] ToHashEntries(KanjiWithData k) => new[]
    {
        new HashEntry("Character",   k.Character.ToString()),
        new HashEntry("KunReadings", k.KunReadings),
        new HashEntry("OnReadings",  k.OnReadings),
        new HashEntry("Meanings",    k.Meanings),
    };
    static string Get(HashEntry[] entries, string name) => entries.FirstOrDefault(e => e.Name == name).Value.ToString();
    
    public async Task SetInitialKanji(IReadOnlyCollection<KanjiWithData> allKanji)
    {
        var db = redis.GetDatabase();
        var upserts = allKanji.Select(k => db.HashSetAsync(KanjiKey(k.Character), ToHashEntries(k)));
        await Task.WhenAll(upserts);
    }
    
    public async Task SetUserKanji(string userId, IReadOnlyCollection<char> chars)
    {
        var db = redis.GetDatabase();
        var key = UserCharsKey(userId);

        await db.KeyDeleteAsync(key);

        if (chars.Count > 0)
        {
            var members = chars.Select(c => (RedisValue)Cp(c)).ToArray();
            await db.SetAddAsync(key, members);
        }
    }

    public async Task<IReadOnlyCollection<KanjiWithData>> GetUserKanji(string userId, CancellationToken cancellationToken)
    {
        var userKanjiCharacters = (await GetUserKanjiCharacters(userId)).ToArray();
        var db = redis.GetDatabase();

        var tasks = userKanjiCharacters.Select(ch => db.HashGetAllAsync(KanjiKey(ch))).ToArray();
        await Task.WhenAll(tasks);
        
        var missing = new List<char>();
        for (int i = 0; i < tasks.Length; i++)
            if (tasks[i].Result.Length == 0) missing.Add(userKanjiCharacters[i]);

        if (missing.Count > 0)
        {
            var fromDb = (await kanjiRepository.GetKanjiByCharacters(missing, cancellationToken)).Select(CommonConverter.Convert);
            var upserts = fromDb.Select(k => db.HashSetAsync(KanjiKey(k.Character), ToHashEntries(k)));
            await Task.WhenAll(upserts);
        }

        var list = new List<KanjiWithData>(tasks.Length);
        foreach (var t in tasks)
        {
            var h = t.Result;
            if (h == null || h.Length == 0) continue;

            var chStr = Get(h, "Character");
            if (string.IsNullOrEmpty(chStr)) continue;

            list.Add(new KanjiWithData
            {
                Character   = chStr[0],
                KunReadings = Get(h, "KunReadings"),
                OnReadings  = Get(h, "OnReadings"),
                Meanings    = Get(h, "Meanings")
            });
        }
        return list;
    }

    public async Task<IReadOnlyCollection<char>> GetUserKanjiCharacters(string userId)
    {
        var db = redis.GetDatabase();
        var members = await db.SetMembersAsync(UserCharsKey(userId));

        return members
            .Select(rv => (string)rv!)
            .Where(s => TryParseCp(s!))
            .Select(CpToChar)
            .ToList();
    }
}
