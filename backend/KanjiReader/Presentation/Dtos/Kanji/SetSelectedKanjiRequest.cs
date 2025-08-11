using System.Text.Json.Serialization;
using KanjiReader.Domain.DomainObjects.KanjiLists;

namespace KanjiReader.Presentation.Dtos.Kanji;

public class SetSelectedKanjiRequest
{
    public char[] Kanji { get; set; }
    public KanjiListType[] KanjiLists { get; set; }
}