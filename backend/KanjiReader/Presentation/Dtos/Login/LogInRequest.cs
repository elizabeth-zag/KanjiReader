using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login;

public class LogInRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}