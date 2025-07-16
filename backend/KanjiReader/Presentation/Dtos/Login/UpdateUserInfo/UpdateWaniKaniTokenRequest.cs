using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class UpdateWaniKaniTokenRequest
{
    [Required]
    public string Token { get; set; }
}