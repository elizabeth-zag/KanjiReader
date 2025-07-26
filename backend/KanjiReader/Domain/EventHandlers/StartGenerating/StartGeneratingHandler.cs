using System.Text.Json;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers.StartGenerating;

public class StartGeneratingHandler(IEventRepository eventRepository,
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext) 
    : CommonEventHandler(eventRepository,
        processingResultRepository,
        userAccountService,
        userGenerationStateRepository,
        textService,
        dbContext)
{
    protected override async Task Execute(string userId, string stringData, CancellationToken cancellationToken)
    {
        var data = JsonSerializer.Deserialize<StartGeneratingData>(stringData); // todo: NRE
        var events = data.SourceTypes
            .Select(CommonConverter.ConvertToEventType)
            .Where(st => st != EventType.Unspecified)
            .Select(st => CreateEventsService.CreateNewEvent(userId, st))
            .ToArray();

        await eventRepository.Create(events, cancellationToken);
    }

    protected override Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState state)> ProcessTexts(
        User user, 
        UserGenerationState? generationState, 
        int remainingTextCount,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); // todo: think about service
    }

    protected override EventType GetEventType()
    {
        return EventType.StartGenerating;
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Unspecified;
    }
}