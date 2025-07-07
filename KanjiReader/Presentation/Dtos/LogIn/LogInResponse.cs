namespace KanjiReader.Presentation.Dtos.LogIn;

public class LogInResponse
{
    public string JwtToken { get; set; }
    public LogInResultStatusCode StatusCode { get; set; }
}