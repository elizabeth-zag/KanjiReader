using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader.Contracts;

public class SatoriRun
{
    [JsonPropertyName("parts")]
    public SatoriPart[] Parts { get; set; }
}