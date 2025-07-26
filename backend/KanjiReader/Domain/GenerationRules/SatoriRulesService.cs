using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.DomainObjects.EventData.BaseData;

namespace KanjiReader.Domain.GenerationRules;

public class SatoriRulesService : IGenerationRulesService<SatoriParsingData, SatoriParsingBaseData>
{
    public SatoriParsingData GetNextState(SatoriParsingData? data, SatoriParsingBaseData baseData)
    {
        if (data == null)
        {
            return GetDefault();
        }
        
        var newSeriesNumber = data.SeriesNumber < baseData.MaxSeriesNumber ? data.SeriesNumber + 1 : 0;
        return new SatoriParsingData { SeriesNumber = newSeriesNumber };
    }

    private static SatoriParsingData GetDefault()
    {
        return new SatoriParsingData { SeriesNumber = 0 };
    }
}