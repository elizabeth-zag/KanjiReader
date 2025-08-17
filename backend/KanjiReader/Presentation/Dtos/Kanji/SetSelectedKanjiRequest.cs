using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Kanji;

public class SetSelectedKanjiRequest
{
    public char[] Kanji { get; set; }
    public KanjiListType[] KanjiLists { get; set; }
}