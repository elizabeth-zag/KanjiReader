namespace KanjiReader.Infrastructure.Database.Models.Events;

public class Event
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public EventType Type { get; set; }
    public string Data { get; set; }
    public DateTime ExecutionTime { get; set; }
}