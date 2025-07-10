using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.WaniKani.Contracts;

public class AssignmentData
{
    [JsonPropertyName("subject_id")]
    public int SubjectId { get; set; }
}