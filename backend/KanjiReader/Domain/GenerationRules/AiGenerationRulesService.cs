using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;

namespace KanjiReader.Domain.GenerationRules;

public class AiGenerationRulesService : IGenerationRulesService<AiGenerationData, AiGenerationBaseData>
{
    public AiGenerationData GetNextState(AiGenerationData? data, AiGenerationBaseData baseData)
    {
        if (data is null)
        {
            return new AiGenerationData(0, DateTime.UtcNow, true);
        }

        var isTimePassed = (DateTime.UtcNow - data.LastGenerated).TotalMinutes > baseData.CooldownMinutes;
        if (isTimePassed)
        {
            return new AiGenerationData(0, DateTime.UtcNow, true);
        }

        if (data.TokensSpent < baseData.TokenLimit)
        {
            return new AiGenerationData(data.TokensSpent, DateTime.UtcNow, true);
        }
        
        return new AiGenerationData(data.TokensSpent, data.LastGenerated, false);
    }
}