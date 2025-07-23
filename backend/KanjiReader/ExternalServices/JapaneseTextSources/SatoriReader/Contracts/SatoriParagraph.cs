using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader.Contracts;

public class SatoriParagraph
{
    [JsonPropertyName("sentences")]
    public SatoriSentence[] Sentences { get; set; }
}