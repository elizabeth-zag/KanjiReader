using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.WaniKani.Contracts;

public class ApiPage<T>
{
    [JsonPropertyName("data")]
    public T[]? Data { get; set; }
}