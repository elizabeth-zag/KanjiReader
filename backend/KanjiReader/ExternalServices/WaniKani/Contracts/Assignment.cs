using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.WaniKani.Contracts;

public class Assignment
{
    [JsonPropertyName("data")]
    public AssignmentData Data { get; set; }
}