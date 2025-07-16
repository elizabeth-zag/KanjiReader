using System.Text.Json;
using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class EventRepository : IEventRepository
{
    private readonly KanjiReaderDbContext _dbContext;
    private readonly IMapper _mapper;
    
    
    public EventRepository(KanjiReaderDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Event>> GetByType(EventType eventType, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Events
            .Where(e => e.Type == eventType)
            .Select(e => JsonSerializer.Deserialize<EventDb>(e.Data, JsonSerializerOptions.Default))
            .OfType<EventDb>()
            .ToArrayAsync(cancellationToken);
        
        return _mapper.Map<Event[]>(result);
    }

    public async Task Create(IReadOnlyCollection<Event> events, CancellationToken cancellationToken)
    {
        var eventsDb = _mapper.Map<IEnumerable<EventDb>>(events);
        await _dbContext.Events.AddRangeAsync(eventsDb, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}