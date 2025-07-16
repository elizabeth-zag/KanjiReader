using System.Text.Json.Serialization;
using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Texts;

public class StartGeneratingRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GenerationSourceType[] SourceTypes { get; set; }
}