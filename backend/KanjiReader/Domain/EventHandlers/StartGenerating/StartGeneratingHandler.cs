using System.Text.Json;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KanjiReader.Domain.EventHandlers.StartGenerating;

public class StartGeneratingHandler(IServiceScopeFactory serviceScopeFactory) : CommonEventHandler(serviceScopeFactory)
{
    private static readonly Dictionary<GenerationSourceType, int> SourceTypeTextCountPerEvent = new()
    {
        { GenerationSourceType.GoogleAiGeneration, 1 },
        { GenerationSourceType.Watanoc, 8 },
        { GenerationSourceType.SatoriReader, 2 },
        { GenerationSourceType.Nhk, 4 } // todo: move to config
    };
    private CreateEventsService _createEventsService;
    
    // dependencies
    private IProcessingResultRepository _processingResultRepository;
    private IEventRepository _eventRepository; // we have it in the base class

    protected override async Task Execute(string userId, string stringData, CancellationToken cancellationToken)
    {
        var data = JsonSerializer.Deserialize<StartGeneratingData>(stringData); // todo: NRE
        var events = data.SourceTypes
            .Select(CommonConverter.ConvertToEventType)
            .Where(st => st != EventType.Unspecified)
            .Select(st => CreateEventsService.CreateNewEvent(userId, st))
            .ToArray();

        await _eventRepository.Create(events, cancellationToken);
    }

    protected override Task<(ProcessingResult[] results, UserGenerationState state)> ProcessTexts(
        User user, 
        UserGenerationState? generationState, 
        int remainingTextCount,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); // todo: think about service
    }

    protected override void SetScopedDependencies(IServiceScope scope)
    {
        _createEventsService = scope.ServiceProvider.GetRequiredService<CreateEventsService>();
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