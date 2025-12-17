using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;

namespace KanjiReader.Domain.GenerationRules;

public class NhkRulesService : IGenerationRulesService<NhkParsingData, NhkParsingBaseData>
{
    public NhkParsingData GetNextState(NhkParsingData? data, NhkParsingBaseData baseData)
    {
        return data == null ? GetDefault() : new NhkParsingData(baseData.LastId);
    }

    private static NhkParsingData GetDefault()
    {
        return new NhkParsingData(0);
    }
}