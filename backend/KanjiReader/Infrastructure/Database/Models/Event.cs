using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Database.Models;

public class Event
{
    public int Id { get; init; }
    public string UserId { get; init; }
    public EventType Type { get; init; }
    public string? Data { get; init; }
    public DateTime ExecutionTime { get; init; }
    public DateTime CreationTime { get; init; }
    public int RetryCount { get; init; }
    
    private Event() {}
    
    public Event(
        string userId, 
        EventType type, 
        string? data, 
        DateTime creationTime, 
        DateTime executionTime, 
        int retryCount)
    {
        UserId = userId;
        Type = type;
        Data = data;
        CreationTime = creationTime;
        ExecutionTime = executionTime;
        RetryCount = retryCount;
    }
}