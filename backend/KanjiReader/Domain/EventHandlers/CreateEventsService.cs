using System.Security.Claims;
using System.Text.Json;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers;

public class CreateEventsService(IEventRepository eventRepository, UserAccountService userAccountService)
{
    private Dictionary<string, int> _watanocCategoryPages = new()
    {
        { "japan-fun", 21 },
        { "japan-news", 8 },
        { "simplejapanese", 5 } // todo: config
    };

    public async Task CreateStartGeneratingEvents(ClaimsPrincipal claimsPrincipal, 
        IReadOnlySet<string> sourceTypes, CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaims(claimsPrincipal);
        var data = new StartGeneratingData
        {
            SourceTypes = sourceTypes.Select(CommonConverter.Convert).ToArray(),
        };

        var ev = new Event(
            user.Id,
            EventType.StartGenerating,
            JsonSerializer.Serialize(data),
            DateTime.UtcNow);

        await eventRepository.Create([ev], cancellationToken);
    }
    
    public async Task CreateWatatocEvents(string userId, CancellationToken cancellationToken)
    {
        var data = new List<WatanocParsingData>();

        foreach (var (category, lastPage) in _watanocCategoryPages)
        {
            data.AddRange(Enumerable.Range(1, lastPage)
                .Select(x => new WatanocParsingData
                {
                    Category = category, PageNumber = x
                }));
        }

        var events = data.Select(d => new Event(
            userId,
            EventType.WatanocParsing,
            JsonSerializer.Serialize(d),
            DateTime.UtcNow)).ToArray();

        await eventRepository.Create(events, cancellationToken);
    }
}