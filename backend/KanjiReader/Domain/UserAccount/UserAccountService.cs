using System.Security.Claims;
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

    public UserAccountService(
        UserManager<User> userManager, 
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
    
    public async Task<User> GetById(string userId)
    {
        return await _userManager.FindByIdAsync(userId); // todo: NRE
    }
    
    public async Task<bool> SetWaniKaniToken(ClaimsPrincipal claimsPrincipal, string token)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal); // todo: NRE
        user.WaniKaniToken = token;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }
}