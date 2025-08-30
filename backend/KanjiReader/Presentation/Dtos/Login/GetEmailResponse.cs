using System.ComponentModel.DataAnnotations;

namespace KanjiReader.Presentation.Dtos.Login;

public class GetEmailResponse
{
    [EmailAddress]
    public string Email { get; set; }
}