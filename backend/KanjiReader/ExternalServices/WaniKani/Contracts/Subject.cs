using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.WaniKani.Contracts;

public class Subject 
{
    [JsonPropertyName("data")]
    public SubjectData Data { get; set; }
}