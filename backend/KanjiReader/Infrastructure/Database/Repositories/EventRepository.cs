using System.Text.Json;
using KanjiReader.Infrastructure.Database.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class EventRepository : IEventRepository
{
    private KanjiReaderDbContext _context;

    public EventRepository(KanjiReaderDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Event>> GetAll()
    {
        return await _context.Event
            .Select(e => JsonSerializer.Deserialize<Event>(e.Data, JsonSerializerOptions.Default))
            .OfType<Event>()
            .ToListAsync();
    }

    public async Task Create(IReadOnlyCollection<Event> events)
    {
        await _context.Event.AddRangeAsync(events);
        await _context.SaveChangesAsync();
    }
}