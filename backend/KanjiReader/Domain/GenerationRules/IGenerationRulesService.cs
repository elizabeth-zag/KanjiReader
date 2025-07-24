namespace KanjiReader.Domain.GenerationRules;

public interface IGenerationRulesService<TData>
{
    public TData GetNextState(TData? data);
}