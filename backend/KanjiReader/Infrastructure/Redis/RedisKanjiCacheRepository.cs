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
        var chars = (await GetUserKanjiCharacters(userId)).ToArray();
        var db = redis.GetDatabase();
        
        var hashes = await Task.WhenAll(chars.Select(ch => db.HashGetAllAsync(KanjiKey(ch))));

        var byChar = new Dictionary<char, KanjiWithData>(chars.Length);
        for (int i = 0; i < chars.Length; i++)
        {
            var h = hashes[i];
            if (h is { Length: > 0 })
            {
                var chStr = Get(h, "Character");
                if (!string.IsNullOrEmpty(chStr))
                {
                    byChar[chStr[0]] = new KanjiWithData
                    {
                        Character   = chStr[0],
                        KunReadings = Get(h, "KunReadings"),
                        OnReadings  = Get(h, "OnReadings"),
                        Meanings    = Get(h, "Meanings")
                    };
                }
            }
        }

        var missing = new List<char>(chars.Length);
        foreach (var t in chars)
            if (!byChar.ContainsKey(t)) missing.Add(t);

        if (missing.Count > 0)
        {
            var fromDb = (await kanjiRepository.GetKanjiByCharacters(missing, cancellationToken))
                .Select(CommonConverter.Convert)
                .Where(k => k is not null)
                .ToArray();

            if (fromDb.Length > 0)
            {
                await Task.WhenAll(fromDb.Select(k => db.HashSetAsync(KanjiKey(k.Character), ToHashEntries(k))));
                foreach (var k in fromDb)
                    byChar[k.Character] = k!;
            }
        }

        var result = new List<KanjiWithData>(byChar.Count);
        foreach (var ch in chars)
            if (byChar.TryGetValue(ch, out var k)) result.Add(k);

        return result;
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
