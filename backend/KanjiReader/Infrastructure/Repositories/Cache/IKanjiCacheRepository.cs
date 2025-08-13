namespace KanjiReader.Infrastructure.Repositories.Cache;

public interface IKanjiCacheRepository
{
    Task SetUserKanji(string userId, IReadOnlySet<char> kanji);
    Task<IReadOnlySet<char>> GetUserKanji(string userId);
}