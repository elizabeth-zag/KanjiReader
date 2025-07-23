using System.Text.Json.Serialization;

namespace KanjiReader.ExternalServices.JapaneseTextSources.Nhk.Contracts;

public class NhkNewsData
{
    [JsonPropertyName("news_id")]
    public string NewsId { get; set; }
}