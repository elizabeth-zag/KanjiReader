using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Infrastructure.Repositories;

public interface IProcessingResultRepository
{
    Task Insert(IReadOnlyCollection<ProcessingResult> result, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProcessingResult>> GetByUser(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task<int> GetCountByUser(string userId, CancellationToken cancellationToken);
    Task Delete(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken);
    Task DeleteForUser(string userId, CancellationToken cancellationToken);
    Task DeleteForUserBySourceTypes(
        string userId,
        IReadOnlyCollection<GenerationSourceType> sourceTypes,
        CancellationToken cancellationToken);
}