using System.ComponentModel.DataAnnotations;
using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class TryUpdateKanjiSourceRequest
{
    [Required]
    public KanjiSourceType KanjiSourceType { get; set; }
}