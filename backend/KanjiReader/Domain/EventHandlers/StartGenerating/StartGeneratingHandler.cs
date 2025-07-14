using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KanjiReader.Domain.EventHandlers.StartGenerating;

public class StartGeneratingHandler(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private IEventRepository _eventRepository;
    private CreateEventsService _createEventsService;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            _eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            _createEventsService = scope.ServiceProvider.GetRequiredService<CreateEventsService>();
            
            var events = await _eventRepository.GetByType(EventType.StartGenerating, cancellationToken);

            foreach (var ev in events)
            {
                var data = JsonSerializer.Deserialize<StartGeneratingData>(ev.Data); // todo: NRE
                await Execute(ev.UserId, data, cancellationToken);
            }
            
            await Task.Delay(1000, cancellationToken); // todo: config
        }
    }

    private async Task Execute(string userId, StartGeneratingData data, CancellationToken cancellationToken)
    {
        
        foreach (var sourceType in data.SourceTypes)
        {
            switch (sourceType)
            {
                case GenerationSourceType.Watanoc:
                    await _createEventsService.CreateWatatocEvents(userId, cancellationToken);
                    break;
            }
        }
    }
}