using System.Security.Claims;

namespace KanjiReader.Presentation.Dtos.LogIn;

public class LogInResponse
{
    public ClaimsIdentity? ClaimsIdentity { get; set; }
    public string ErrorMessage { get; set; } = "";
}