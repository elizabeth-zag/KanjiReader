using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login;

public class SendConfirmationCodeRequest
{
    [Required]
    public string UserName { get; set; }
}