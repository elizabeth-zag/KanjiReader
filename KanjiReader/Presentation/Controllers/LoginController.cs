using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.LogIn;
using KanjiReader.Presentation.Dtos.Register;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Route("api/Login")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly UserAccountService _userAccountService;
    
    public LoginController(
        IConfiguration config, UserAccountService userAccountService)
    {
        _config = config;
        _userAccountService = userAccountService;
    }
    
    [HttpPost("register")]
    public async Task<RegisterResponse> Register(RegisterRequest dto)
    {
        return await _userAccountService.Register(dto, DateTime.UtcNow);
    }

    
    [HttpPost("login")]
    public async Task<LogInResponse> LogIn(LogInRequest dto)
    {
        return await _userAccountService.LogIn(dto, DateTime.UtcNow);
    }
}