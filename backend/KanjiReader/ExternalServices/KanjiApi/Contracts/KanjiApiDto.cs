using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.KanjiApi.Contracts;

public class KanjiApiDto
{
    [JsonPropertyName("kanji")]
    public char Kanji { get; set; }
    [JsonPropertyName("kun_readings")]
    public string[] KunReadings { get; set; }
    [JsonPropertyName("on_readings")]
    public string[] OnReadings { get; set; }
    [JsonPropertyName("meanings")]
    public string[] Meanings { get; set; }
}