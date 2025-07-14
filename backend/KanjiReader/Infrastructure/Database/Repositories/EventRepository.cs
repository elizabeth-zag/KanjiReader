using System.Text.Json;
using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class EventRepository : IEventRepository
{
    private readonly KanjiReaderDbContext _context;
    private readonly IMapper _mapper;
    
    public EventRepository(KanjiReaderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Event>> GetByType(EventType eventType, CancellationToken cancellationToken)
    {
        var result = await _context.Events
            .Select(e => JsonSerializer.Deserialize<EventDb>(e.Data, JsonSerializerOptions.Default))
            .OfType<EventDb>()
            .Where(e => e.Type == eventType)
            .ToArrayAsync(cancellationToken);
        
        return _mapper.Map<Event[]>(result);
    }

    public async Task Create(IReadOnlyCollection<Event> events, CancellationToken cancellationToken)
    {
        var eventsDb = _mapper.Map<IEnumerable<EventDb>>(events);
        await _context.Events.AddRangeAsync(eventsDb, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}