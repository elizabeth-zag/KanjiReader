using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class UpdateNameRequest
{
    [Required]
    public string Name { get; set; }
}