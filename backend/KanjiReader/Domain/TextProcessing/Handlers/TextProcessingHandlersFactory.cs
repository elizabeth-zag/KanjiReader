using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.TextProcessing.Handlers.GoogleAiGeneration;
using KanjiReader.Domain.TextProcessing.Handlers.NhkParsing;
using KanjiReader.Domain.TextProcessing.Handlers.SatoriParsing;
using KanjiReader.Domain.TextProcessing.Handlers.WatanocParsing;

namespace KanjiReader.Domain.TextProcessing.Handlers;

public class TextProcessingHandlersFactory(
    WatanocParsingHandler watanocParsingHandler,
    NhkParsingHandler nhkParsingHandler,
    GoogleAiGenerationHandler googleAiGenerationHandler,
    SatoriParsingHandler satoriParsingHandler)
{
    public CommonTextProcessingHandler GetHandler(GenerationSourceType sourceType)
    {
        return sourceType switch
        {
            GenerationSourceType.Watanoc => watanocParsingHandler,
            GenerationSourceType.SatoriReader => satoriParsingHandler,
            GenerationSourceType.Nhk => nhkParsingHandler,
            GenerationSourceType.GoogleAiGeneration => googleAiGenerationHandler,
            _ => throw new ArgumentOutOfRangeException(nameof(GenerationSourceType), sourceType, null)
        };
    }
}