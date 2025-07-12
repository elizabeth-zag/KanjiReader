using System.Text.Json;
using KanjiReader.Infrastructure.Database.Models.Events;
using KanjiReader.Infrastructure.Database.Repositories;

namespace KanjiReader.Domain.EventHandler;

public class StartGeneratingHandler
{
    private Dictionary<string, int> _watanocCategoryPages = new()
    {
        { "japan-fun", 21 },
        { "japan-news", 8 },
        { "simplejapanese", 5 } // todo: config
    };
    private IEventRepository _eventRepository;

    public StartGeneratingHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task Execute(string userId, StartGeneratingData data)
    {
        foreach (var sourceType in data.SourceTypes)
        {
            switch (sourceType)
            {
                case GenerationSourceType.Watanoc:
                    await CreateWatatocTasks(userId);
                    break;
            }
        }
    }

    private async Task CreateWatatocTasks(string userId)
    {
        var data = new List<ParseWatanocData>();

        foreach (var (category, lastPage) in _watanocCategoryPages)
        {
            data.AddRange(Enumerable.Range(1, lastPage)
                .Select(x => new ParseWatanocData
                {
                    Category = category, PageNumber = x
                }));
        }

        var events = data.Select(d => new Event
        {
            UserId = userId,
            Data = JsonSerializer.Serialize(d),
            Type = EventType.WatanocParsing,
            ExecutionTime = DateTime.UtcNow
        }).ToArray();

        await _eventRepository.Create(events);
    }
}