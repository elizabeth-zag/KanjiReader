using KanjiReader.Domain.DomainObjects.EventData;

namespace KanjiReader.Domain.GenerationRules;

public class NhkRulesService : IGenerationRulesService<NhkParsingData>
{
    public NhkParsingData GetNextState(NhkParsingData? data)
    {
        if (data == null)
        {
            return CreateNewState();
        }
        var nextDate = data.OrderedDates.LastOrDefault(d => d > data.FirstDate);
        if (nextDate != default)
        {
            return new NhkParsingData
            {
                CurrentDate = nextDate,
                FirstDate = nextDate,
                LastDate = data.LastDate,
                OrderedDates = data.OrderedDates
            };
        }
        
        nextDate = data.OrderedDates.FirstOrDefault(d => d < data.LastDate);
        if (nextDate != default)
        {
            return new NhkParsingData
            {
                CurrentDate = nextDate,
                FirstDate = data.FirstDate,
                LastDate = nextDate,
                OrderedDates = data.OrderedDates
            };
        }
        
        return new NhkParsingData
        {
            CurrentDate = data.OrderedDates.First(),
            FirstDate = data.OrderedDates.First(),
            LastDate = data.OrderedDates.First(),
            OrderedDates = data.OrderedDates
        };
    }

    private NhkParsingData CreateNewState()
    {
        return new NhkParsingData
        {
            CurrentDate = DateTime.UtcNow,
            FirstDate = DateTime.UtcNow,
            LastDate = DateTime.UtcNow
        };
    }
}