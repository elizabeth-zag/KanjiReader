namespace KanjiReader.Presentation.Dtos.Register;

public class RegisterResponse
{
    public RegistrationResultStatusCode StatusCode { get; set; }
    public string ErrorMessage { get; set; } = "";
}