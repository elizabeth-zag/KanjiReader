using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class EventRepository : IEventRepository
{
    private readonly KanjiReaderDbContext _dbContext;
    
    public EventRepository(KanjiReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Event>> GetByType(EventType eventType, CancellationToken cancellationToken)
    {
        return await _dbContext.Events
            .Where(e => e.Type == eventType)
            .Select(e => JsonSerializer.Deserialize<Event>(e.Data, JsonSerializerOptions.Default))
            .OfType<Event>()
            .ToArrayAsync(cancellationToken);
    }

    public async Task Create(IReadOnlyCollection<Event> events, CancellationToken cancellationToken)
    {
        await _dbContext.Events.AddRangeAsync(events, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(IReadOnlyCollection<Event> events, CancellationToken cancellationToken)
    {
        await _dbContext.Events
            .Where(e => events.Any(ev => ev.Id == e.Id))
            .ExecuteDeleteAsync(cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}