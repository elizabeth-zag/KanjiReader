using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login;

public class RegisterRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}