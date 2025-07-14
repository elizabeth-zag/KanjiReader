namespace KanjiReader.Domain.DomainObjects;

public class Event
{
    public int Id;
    public string UserId;
    public EventType Type;
    public string Data;
    public DateTime ExecutionTime;

    public Event(string userId, EventType type, string data, DateTime executionTime)
    {
        UserId = userId;
        Type = type;
        Data = data;
        ExecutionTime = executionTime;
    }
}