using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.LogIn;
using KanjiReader.Presentation.Dtos.Register;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly UserAccountService _userAccountService;
    
    public LoginController(UserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }
    
    [HttpPost(nameof(Register))]
    public async Task<RegisterResponse> Register(RegisterRequest dto)
    {
        return await _userAccountService.Register(dto, DateTime.UtcNow);
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
    
    // [Authorize]
    [HttpGet(nameof(GetCurrentUser))]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            userName = User.Identity?.Name ?? string.Empty,
        });
    }
}