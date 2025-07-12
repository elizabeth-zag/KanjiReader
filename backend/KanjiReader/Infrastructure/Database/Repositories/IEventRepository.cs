using KanjiReader.Infrastructure.Database.Models.Events;

namespace KanjiReader.Infrastructure.Database.Repositories;

public interface IEventRepository
{
    Task<IReadOnlyCollection<Event>> GetAll();
    Task Create(IReadOnlyCollection<Event> @event);
}