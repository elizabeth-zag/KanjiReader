using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.TextProcessing.Handlers.NhkParsing;
using KanjiReader.Domain.TextProcessing.Handlers.SatoriParsing;
using KanjiReader.Domain.TextProcessing.Handlers.WatanocParsing;

namespace KanjiReader.Domain.TextProcessing.Handlers;

public class TextProcessingHandlersFactory(
    WatanocParsingHandler watanocParsingHandler,
    NhkParsingHandler nhkParsingHandler,
    SatoriParsingHandler satoriParsingHandler)
{
    public CommonTextProcessingHandler GetHandler(GenerationSourceType sourceType)
    {
        return sourceType switch
        {
            GenerationSourceType.Watanoc => watanocParsingHandler,
            GenerationSourceType.SatoriReader => satoriParsingHandler,
            GenerationSourceType.Nhk => nhkParsingHandler,
            _ => throw new ArgumentOutOfRangeException(nameof(GenerationSourceType), sourceType, null)
        };
    }
}