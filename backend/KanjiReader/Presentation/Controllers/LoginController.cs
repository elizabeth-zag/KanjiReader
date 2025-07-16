using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.Kanji;
using KanjiReader.Presentation.Dtos.Login;
using KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly UserAccountService _userAccountService;
    private readonly WaniKaniService _waniKaniService;
    
    public LoginController(UserAccountService userAccountService, WaniKaniService waniKaniService)
    {
        _userAccountService = userAccountService;
        _waniKaniService = waniKaniService;
    }
    
    [HttpPost(nameof(Register))]
    public async Task<RegisterResponse> Register(RegisterRequest dto, CancellationToken cancellationToken)
    {
        var response = await _userAccountService.Register(dto, DateTime.UtcNow);
        
        if (!string.IsNullOrEmpty(dto.WaniKaniToken))
        {
            try
            {
                await _waniKaniService.FillWaniKaniKanjiCache(User, dto.WaniKaniToken, cancellationToken);
            }
            catch { } // create some logging
        }
        
        return response;
    }
    
    [HttpPost(nameof(LogIn))]
    public async Task<IActionResult> LogIn(LogInRequest dto)
    {
        var errorMessage = await _userAccountService.LogIn(dto, DateTime.UtcNow);
        
        if (string.IsNullOrEmpty(errorMessage))
        {
            return Unauthorized(new { message = errorMessage });
        }
        return Ok();
    }
    
    [HttpPost(nameof(LogOut))]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out" });
    }
    
    [Authorize]
    [HttpGet(nameof(GetCurrentUser))]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            userName = User.Identity?.Name ?? string.Empty,
        });
    }
    
    [Authorize]
    [HttpPost(nameof(SetWaniKaniToken))]
    public async Task SetWaniKaniToken(UpdateWaniKaniTokenRequest dto, CancellationToken cancellationToken)
    {
        await _userAccountService.UpdateWaniKaniToken(User, dto.Token);

        try
        {
            await _waniKaniService.FillWaniKaniKanjiCache(User, dto.Token, cancellationToken);
        }
        catch { } // create some logging
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateName))]
    public async Task<IActionResult> UpdateName(UpdateNameRequest dto)
    {
        await _userAccountService.UpdateName(User, dto.Name);
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateKanjiSourceType))]
    public async Task<IActionResult> UpdateKanjiSourceType(UpdateKanjiSourceTypeRequest dto)
    {
        await _userAccountService.UpdateKanjiSourceType(User, dto.KanjiSourceType);
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateEmail))]
    public async Task<IActionResult> UpdateEmail(UpdateEmailRequest dto)
    {
        await _userAccountService.UpdateEmail(User, dto.Email);
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(UpdatePassword))]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest dto)
    {
        await _userAccountService.UpdatePassword(User, dto.OldPassword, dto.NewPassword);
        return Ok();
    }
}