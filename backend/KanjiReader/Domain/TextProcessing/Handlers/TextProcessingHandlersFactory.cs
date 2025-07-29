using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.TextProcessing.Handlers.NhkParsing;
using KanjiReader.Domain.TextProcessing.Handlers.SatoriParsing;
using KanjiReader.Domain.TextProcessing.Handlers.WatanocParsing;

namespace KanjiReader.Domain.TextProcessing.Handlers;

public class TextProcessingHandlersFactory
{
    private readonly WatanocParsingHandler _watanocParsingHandler;
    private readonly NhkParsingHandler _nhkParsingHandler;
    private readonly SatoriParsingHandler _satoriParsingHandler;

    public TextProcessingHandlersFactory(
        WatanocParsingHandler watanocParsingHandler, 
        NhkParsingHandler nhkParsingHandler, 
        SatoriParsingHandler satoriParsingHandler)
    {
        _watanocParsingHandler = watanocParsingHandler;
        _nhkParsingHandler = nhkParsingHandler;
        _satoriParsingHandler = satoriParsingHandler;
    }

    public CommonTextProcessingHandler GetHandler(GenerationSourceType sourceType)
    {
        return sourceType switch
        {
            GenerationSourceType.Watanoc => _watanocParsingHandler,
            GenerationSourceType.SatoriReader => _satoriParsingHandler,
            GenerationSourceType.Nhk => _nhkParsingHandler,
            _ => throw new ArgumentOutOfRangeException(nameof(GenerationSourceType), sourceType, null)
        };
    }
}