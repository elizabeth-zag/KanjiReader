using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/Login")]
public class LoginController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    
    public LoginController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }
    
    [HttpPost("register")]
    public void Register(RegisterRequest dto)
    {
        
    }
    
    [HttpPost("login")]
    public void LogIn(LogInRequest dto)
    {
        
    }

    [HttpPost("logout")]
    public void LogOut()
    {

    }
}