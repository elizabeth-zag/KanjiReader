using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.LogIn;
using KanjiReader.Presentation.Dtos.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RegisterRequest = KanjiReader.Presentation.Dtos.Register.RegisterRequest;

namespace KanjiReader.Domain.UserAccount;

// todo: add validation
public class UserAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public UserAccountService(
        UserManager<User> userManager, 
        SignInManager<User> signInManager, 
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<RegisterResponse> Register(RegisterRequest dto, DateTime loginTime)
    {
        try
        {
            var user = UserAccountConverter.Convert(dto, loginTime);
            var result = await _userManager.CreateAsync(user, dto.Password);
            return UserAccountConverter.Convert(result);
        }
        catch (Exception ex)
        {
            // todo: exception middleware
            return new RegisterResponse { StatusCode = RegistrationResultStatusCode.ServerError, ErrorMessage = ex.Message };
        }
    }

    public async Task<string> LogIn(LogInRequest dto, DateTime loginTime) // todo: update login time
    {
        try
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
                return "Invalid username or password";

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return "Invalid username or password";
            
            await _signInManager.SignInAsync(user, isPersistent: true);
            
            return string.Empty;
        }
        catch (Exception ex)
        {
            // todo: exception middleware
            return ex.Message;
        }
    }

    // public async Task<LogInResponse> LogInWithJWT(LogInRequest dto, DateTime loginTime) // second option for nowd
    // {
    //     var claims = new[]
    //     {
    //         new Claim(JwtRegisteredClaimNames.Sub, "user.Id"),
    //         new Claim(JwtRegisteredClaimNames.UniqueName, "user.UserName")
    //     };
    //     
    //     // Generate JWT token
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //     
    //     var token = new JwtSecurityToken(
    //         issuer: _configuration["Jwt:Issuer"],
    //         audience: _configuration["Jwt:Audience"],
    //         claims: claims,
    //         expires: DateTime.Now.AddHours(2),
    //         signingCredentials: creds
    //     );
    //     
    //     var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    // }
}