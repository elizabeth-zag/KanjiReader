using KanjiReader.Domain.Deletion;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.Login;
using KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController(
    UserAccountService userAccountService,
    WaniKaniService waniKaniService,
    DeletionService deletionService,
    ILogger<LoginController> logger)
    : ControllerBase
{
    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register(RegisterRequest dto)
    {
        await userAccountService.Register(dto, DateTime.UtcNow);

        return Ok();
    }
    
    [HttpPost(nameof(LogIn))]
    public async Task<IActionResult> LogIn(LogInRequest dto, CancellationToken cancellationToken)
    {
        var loggedInUser = await userAccountService.LogIn(dto, DateTime.UtcNow);
        if (loggedInUser == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }
        
        if (!string.IsNullOrEmpty(loggedInUser.WaniKaniToken))
        {
            try
            {
                await waniKaniService.FillWaniKaniKanjiCache(loggedInUser, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis filling failed");
            }
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
        await userAccountService.UpdateWaniKaniToken(User, dto.Token);
        var user = await userAccountService.GetByClaims(User);

        try
        {
            await waniKaniService.FillWaniKaniKanjiCache(user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis filling failed");
        }
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateName))]
    public async Task<IActionResult> UpdateName(UpdateNameRequest dto)
    {
        await userAccountService.UpdateName(User, dto.Name);
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateKanjiSourceType))]
    public async Task<IActionResult> UpdateKanjiSourceType(UpdateKanjiSourceTypeRequest dto)
    {
        await userAccountService.UpdateKanjiSourceType(User, dto.KanjiSourceType);
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateEmail))]
    public async Task<IActionResult> UpdateEmail(UpdateEmailRequest dto)
    {
        await userAccountService.UpdateEmail(User, dto.Email);
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