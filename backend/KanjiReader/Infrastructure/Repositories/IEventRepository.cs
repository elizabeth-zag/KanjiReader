using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Repositories;

public interface IEventRepository
{
    Task<IReadOnlyCollection<Event>> GetByType(EventType eventType, CancellationToken cancellationToken);
    Task Create(IReadOnlyCollection<Event> events, CancellationToken cancellationToken);
}