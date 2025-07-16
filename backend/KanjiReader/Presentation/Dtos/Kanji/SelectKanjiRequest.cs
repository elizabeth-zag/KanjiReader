using System.Text.Json.Serialization;
using KanjiReader.Domain.DomainObjects.KanjiLists;

namespace KanjiReader.Presentation.Dtos.Kanji;

public class SelectKanjiRequest
{
    public char[] Kanji { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public KanjiListType[] KanjiLists { get; set; }
}