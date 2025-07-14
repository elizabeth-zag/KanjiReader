using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KanjiReader.Domain.EventHandlers.StartGenerating;

public class StartGeneratingHandler(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private Dictionary<string, int> _watanocCategoryPages = new()
    {
        { "japan-fun", 21 },
        { "japan-news", 8 },
        { "simplejapanese", 5 } // todo: config
    };
    private IEventRepository _eventRepository;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            _eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            
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
                    await CreateWatatocTasks(userId, cancellationToken);
                    break;
            }
        }
    }

    private async Task CreateWatatocTasks(string userId, CancellationToken cancellationToken)
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

        await _eventRepository.Create(events, cancellationToken);
    }
}