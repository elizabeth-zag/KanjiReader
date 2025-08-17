using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class SetWaniKaniTokenRequest
{
    [Required]
    public string Token { get; set; }
}