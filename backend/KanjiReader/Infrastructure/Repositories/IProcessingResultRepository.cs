using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Repositories;

public interface IProcessingResultRepository
{
    Task Insert(IReadOnlyCollection<ProcessingResult> result, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProcessingResult>> GetByUser(
        string userId,
        bool isRemoved,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task SetRemoved(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken);
}