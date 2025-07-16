using KanjiReader.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore.Storage;

namespace KanjiReader.Infrastructure.Repositories;

public interface IKanjiRepository
{
    Task<IReadOnlyCollection<Kanji>> GetKanjiByCharacters(IReadOnlyCollection<char> kanji, CancellationToken cancellationToken);
    Task ClearUserKanji(string userId, CancellationToken cancellationToken);
    Task InsertUserKanji(string userId, IReadOnlyCollection<Kanji> kanji, CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}