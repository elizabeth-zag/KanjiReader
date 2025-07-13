using StackExchange.Redis;

namespace KanjiReader.Infrastructure.Repositories;

public interface IKanjiRepository
{
    Task SetUserKanji(string userId, char[] kanji);
    Task<char[]> GetUserKanji(string userId);
}