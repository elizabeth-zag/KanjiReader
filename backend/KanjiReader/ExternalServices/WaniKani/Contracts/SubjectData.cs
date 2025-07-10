using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.WaniKani.Contracts;

public class SubjectData
{
    [JsonPropertyName("characters")]
    public char Characters { get; set; }
}