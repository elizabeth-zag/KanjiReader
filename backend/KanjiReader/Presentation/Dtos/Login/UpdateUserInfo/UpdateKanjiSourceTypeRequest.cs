using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class UpdateKanjiSourceTypeRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public KanjiSourceType KanjiSourceType { get; set; }
}