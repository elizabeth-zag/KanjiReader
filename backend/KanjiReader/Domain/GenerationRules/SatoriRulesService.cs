using KanjiReader.Domain.DomainObjects.EventData;

namespace KanjiReader.Domain.GenerationRules;

public class SatoriRulesService : IGenerationRulesService<SatoriParsingData>
{
    public SatoriParsingData GetNextState(SatoriParsingData? data)
    {
        if (data == null)
        {
            return CreateNewState();
        }
        
        var newSeriesNumber = data.SeriesNumber < data.MaxSeriesNumber ? data.SeriesNumber + 1 : 0;
        return new SatoriParsingData { SeriesNumber = newSeriesNumber };
    }

    private SatoriParsingData CreateNewState()
    {
        return new SatoriParsingData { SeriesNumber = 0 };
    }
}