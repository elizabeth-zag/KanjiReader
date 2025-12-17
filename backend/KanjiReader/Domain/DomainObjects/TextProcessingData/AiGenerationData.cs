namespace KanjiReader.Domain.DomainObjects.TextProcessingData;

public class AiGenerationData(int tokensSpent, DateTime lastGenerated, bool isAllowed)
{
    public int TokensSpent { get; private set;  } = tokensSpent;
    public DateTime LastGenerated { get; private set; } = lastGenerated;
    public bool IsAllowed { get; private set; } = isAllowed;

    public void AddTokens(int tokens)
    {
        TokensSpent += tokens;
        LastGenerated = DateTime.UtcNow;
    }
}