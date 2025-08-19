using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class UpdateEmailRequest
{
    [EmailAddress]
    public string? Email { get; set; }
    public bool NeedDelete { get; set; }
}