using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.WaniKaniToken;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Route("api/kanji")]
public class KanjiController : ControllerBase
{
    private readonly UserAccountService _userAccountService;

    public KanjiController(UserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    [HttpPost(nameof(SetWaniKaniToken))]
    public async Task SetWaniKaniToken(SetWaniKaniTokenRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
        {
            throw new ArgumentException("WaniKani token is empty.", nameof(dto.Token));
        }
        
        await _userAccountService.SetWaniKaniToken(User, dto.Token);
    }
}