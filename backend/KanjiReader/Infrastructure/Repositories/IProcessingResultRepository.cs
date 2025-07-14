using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Repositories;

public interface IProcessingResultRepository
{
    Task Insert(IReadOnlyCollection<ProcessingResult> result, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProcessingResult>> GetByUser(string userId, CancellationToken cancellationToken);
}