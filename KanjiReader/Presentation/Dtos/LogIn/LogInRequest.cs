using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.LogIn;

public class LogInRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}