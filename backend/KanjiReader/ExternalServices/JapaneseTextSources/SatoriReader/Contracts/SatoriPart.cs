using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader.Contracts;

public class SatoriPart
{
    [JsonPropertyName("parts")]
    public SatoriPart[] SatoriTexts { get; set; }
    
    [JsonPropertyName("text")]
    public string Text { get; set; }
}