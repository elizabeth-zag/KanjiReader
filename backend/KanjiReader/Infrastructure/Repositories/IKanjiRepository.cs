using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Infrastructure.Repositories;

public interface IKanjiRepository
{
    Task<IReadOnlyCollection<Kanji>> GetKanjiByCharacters(IReadOnlyCollection<char> kanji, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Kanji>> GetKanjiByUser(string userId, CancellationToken cancellationToken);
    Task ClearUserKanji(string userId, CancellationToken cancellationToken);
    Task InsertUserKanji(string userId, IReadOnlyCollection<Kanji> kanji, CancellationToken cancellationToken);
}