namespace KanjiReader.Presentation.Dtos.Login;

public class LogInResponse
{
    public string ErrorMessage { get; set; } = "";
    public bool NeedEmailConfirmation { get; set; }
}