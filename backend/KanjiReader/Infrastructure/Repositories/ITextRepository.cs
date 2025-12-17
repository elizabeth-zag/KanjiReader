using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Infrastructure.Repositories;

public interface ITextRepository
{
    Task Insert(Text text, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Text>> GetAllBySourceType(
        GenerationSourceType type,
        CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Text>> GetBySourceType(
        GenerationSourceType type,
        int lastId,
        int take,
        CancellationToken cancellationToken);

}