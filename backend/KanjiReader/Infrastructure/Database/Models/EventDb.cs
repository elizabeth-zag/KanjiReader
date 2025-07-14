using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Database.Models;

public class EventDb
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public EventType Type { get; set; }
    public string Data { get; set; }
    public DateTime ExecutionTime { get; set; }
}