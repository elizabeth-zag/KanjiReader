using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;

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
        return new SatoriParsingData(newSeriesNumber);
    }

    private static SatoriParsingData GetDefault()
    {
        return new SatoriParsingData(0);
    }
}