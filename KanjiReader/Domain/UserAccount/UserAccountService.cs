using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.LogIn;
using KanjiReader.Presentation.Dtos.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RegisterRequest = KanjiReader.Presentation.Dtos.Register.RegisterRequest;

namespace KanjiReader.Domain.UserAccount;

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

    public async Task<LogInResponse> LogIn(LogInRequest dto, DateTime loginTime)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
                return UserAccountConverter.Convert(LogInResultStatusCode.InvalidCredentials, String.Empty);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return UserAccountConverter.Convert(LogInResultStatusCode.InvalidCredentials, String.Empty);

            // Generate JWT token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            // todo: add validation for nullable fields
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return UserAccountConverter.Convert(LogInResultStatusCode.Success, jwt);
        }
        catch (Exception ex)
        {
            // todo: exception middleware
            return new LogInResponse { StatusCode = LogInResultStatusCode.ServerError };
        }
    }
}