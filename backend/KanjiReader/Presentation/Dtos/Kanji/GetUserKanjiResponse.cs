using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Kanji;

public class GetUserKanjiResponse
{
    public KanjiWithData[] Kanji { get; set; }
    public string KanjiSourceType { get; set; }
}