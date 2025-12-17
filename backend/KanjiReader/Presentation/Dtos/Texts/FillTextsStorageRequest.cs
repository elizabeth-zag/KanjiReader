using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class FillTextsStorageRequest
{
    public GenerationSourceType SourceType { get; set; }
    public string AuthCookie { get; set; }
}