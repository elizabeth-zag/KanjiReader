using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader.Contracts;

public class SatoriArticleResponse
{
    [JsonPropertyName("paragraphs")]
    public SatoriParagraph[] Paragraphs { get; set; }
}