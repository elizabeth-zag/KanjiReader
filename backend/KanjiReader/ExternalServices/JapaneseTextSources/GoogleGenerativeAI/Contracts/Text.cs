using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.GoogleGenerativeAI.Contracts;

public class Text
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
}