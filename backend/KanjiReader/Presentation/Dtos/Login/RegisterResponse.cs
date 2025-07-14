namespace KanjiReader.Presentation.Dtos.Login;

public class RegisterResponse
{
    public RegistrationResultStatusCode StatusCode { get; set; }
    public string ErrorMessage { get; set; } = "";
}