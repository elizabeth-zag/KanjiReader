using System.Security.Claims;
using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers;

public class CreateEventsService(
    IEventRepository eventRepository,
    UserAccountService userAccountService,
    IProcessingResultRepository processingResultRepository)
{
    public async Task CreateStartGeneratingEvents(ClaimsPrincipal claimsPrincipal, 
        IReadOnlySet<GenerationSourceType> sourceTypes, CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaims(claimsPrincipal);
        
        var textCountLimit = 30; // todo: move to config
        var currentTextCount = await processingResultRepository.GetCountByUser(user.Id, cancellationToken);
        
        if (currentTextCount >= textCountLimit)
        {
            throw new InvalidOperationException($"You have reached the limit of {textCountLimit} texts. Please delete some texts before generating new ones.");
        }
        
        var data = new StartGeneratingData
        {
            SourceTypes = sourceTypes.Where(t => t != GenerationSourceType.Unspecified).ToArray(),
        };

        var ev = CreateNewEvent(user.Id, EventType.StartGenerating, JsonSerializer.Serialize(data));

        await eventRepository.Create([ev], cancellationToken);
    }
    
    public static Event CreateNewEvent(string userId, EventType eventType)
    {
        return CreateNewEvent(userId, eventType, null);
    }
    
    public static Event CreateRetryEvent(Event eventForRetry)
    {
        return new Event(
            eventForRetry.UserId,
            eventForRetry.Type,
            eventForRetry.Data,
            eventForRetry.CreationTime,
            DateTime.UtcNow.AddMinutes(GetRetryDelayMinutes(eventForRetry.RetryCount)),
            eventForRetry.RetryCount + 1);
    }
    
    private static Event CreateNewEvent(string userId, EventType eventType, string? data)
    {
        return new Event(
            userId,
            eventType,
            data,
            DateTime.UtcNow,
            DateTime.UtcNow,
            0);
    }
    
    private static int GetRetryDelayMinutes(int retryCount) // todo: config
    {
        return retryCount switch
        {
            0 => 10,
            1 => 60,
            2 => 1440,
            _ => 10080
        };
    }
}