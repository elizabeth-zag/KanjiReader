namespace KanjiReader.Domain.GenerationRules;

public interface IGenerationRulesService<TData, TBaseData>
{
    public TData GetNextState(TData? data, TBaseData baseData);
}