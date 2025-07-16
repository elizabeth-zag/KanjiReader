using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class UpdateEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}