using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;

namespace KanjiReader.Domain.GenerationRules;

public class NhkRulesService : IGenerationRulesService<NhkParsingData, NhkParsingBaseData>
{
    public NhkParsingData GetNextState(NhkParsingData? data, NhkParsingBaseData baseData)
    {
        if (data == null)
        {
            return GetDefault(baseData);
        }
        
        var nextDate = baseData.OrderedDates.LastOrDefault(d => d > data.LastDate);
        if (nextDate != default)
        {
            return new NhkParsingData
            {
                CurrentDate = nextDate,
                FirstDate = data.FirstDate,
                LastDate = nextDate
            };
        }
        
        nextDate = baseData.OrderedDates.FirstOrDefault(d => d < data.FirstDate);
        if (nextDate != default)
        {
            return new NhkParsingData
            {
                CurrentDate = nextDate,
                FirstDate = nextDate,
                LastDate = data.LastDate
            };
        }

        return GetDefault(baseData);
    }

    private static NhkParsingData GetDefault(NhkParsingBaseData baseData)
    {
        return new NhkParsingData
        {
            CurrentDate = baseData.OrderedDates.First(),
            FirstDate = baseData.OrderedDates.First(),
            LastDate = baseData.OrderedDates.First()
        };
    }
}