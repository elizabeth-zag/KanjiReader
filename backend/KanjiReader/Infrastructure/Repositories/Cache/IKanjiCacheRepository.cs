using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Repositories.Cache;

public interface IKanjiCacheRepository
{
    Task SetInitialKanji(IReadOnlyCollection<KanjiWithData> kanji);
    Task SetUserKanji(string userId, IReadOnlyCollection<char> kanji);
    Task<IReadOnlyCollection<KanjiWithData>> GetUserKanji(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<char>> GetUserKanjiCharacters(string userId);
}