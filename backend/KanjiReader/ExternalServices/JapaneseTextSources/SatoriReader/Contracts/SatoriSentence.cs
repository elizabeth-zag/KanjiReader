using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader.Contracts;

public class SatoriSentence
{
    [JsonPropertyName("runs")]
    public SatoriRun[] Runs { get; set; }
}