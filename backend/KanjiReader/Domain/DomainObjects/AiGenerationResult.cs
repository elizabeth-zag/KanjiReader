namespace KanjiReader.Domain.DomainObjects;

public record struct AiGenerationResult(
    string Title,
    string Content,
    int TokensSpent);