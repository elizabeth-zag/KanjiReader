namespace KanjiReader.Presentation.Dtos.Login;

public class RegisterResponse // todo: ???
{
    public RegistrationResultStatusCode StatusCode { get; set; }
    public string ErrorMessage { get; set; } = "";
}