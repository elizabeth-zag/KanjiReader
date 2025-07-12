using KanjiReader.Infrastructure.Database.Models.Events;
using KanjiReader.Infrastructure.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KanjiReader.Domain.EventHandler;

public class EventHandlerService : BackgroundService
{
    private readonly IServiceScopeFactory  _serviceScopeFactory;

    public EventHandlerService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            IEventRepository eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            // await _eventRepository.Create(new StartGeneratingData { SourceTypes = [GenerationSourceType.Watanoc] });
            
            var events = await eventRepository.GetAll();
            var batchSize = 10; // todo: config
            var batchCount = 0;

            foreach (var @event in events.Skip(batchCount * batchSize).Take(batchSize))
            {
                
                
            }
            
            await Task.Delay(1000, stoppingToken); // todo: config
        }
    }
}