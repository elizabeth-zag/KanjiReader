using System.Security.Claims;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.Login;
using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Domain.UserAccount;

// todo: add validation
public class UserAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private User? CurrentUser;

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
            var user = CommonConverter.Convert(dto, loginTime);
            var result = await _userManager.CreateAsync(user, dto.Password);
            return CommonConverter.Convert(result);
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
        if (CurrentUser == null)
        {
            CurrentUser = await _userManager.FindByIdAsync(userId); // todo: NRE
        }
        return CurrentUser; 
    }
    
    public async Task<User> GetByClaims(ClaimsPrincipal claimsPrincipal)
    {
        if (CurrentUser == null)
        {
            CurrentUser = await _userManager.GetUserAsync(claimsPrincipal); // todo: NRE
        }

        return CurrentUser;
    }
    
    public async Task<bool> UpdateWaniKaniToken(ClaimsPrincipal claimsPrincipal, string token)
    {
        var user = await GetByClaims(claimsPrincipal);
        user.WaniKaniToken = token;
        user.KanjiSourceType = KanjiSourceType.WaniKani;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded; // todo: handle errors
    }
    
    public async Task UpdateKanjiSourceType(ClaimsPrincipal claimsPrincipal, KanjiSourceType kanjiSourceType)
    {
        var user = await GetByClaims(claimsPrincipal);
        user.KanjiSourceType = kanjiSourceType;
        await _userManager.UpdateAsync(user);
    }
    
    public async Task UpdateName(ClaimsPrincipal claimsPrincipal, string name)
    {
        var user = await GetByClaims(claimsPrincipal);
        user.UserName = name;
        await _userManager.UpdateAsync(user);
    }
    
    public async Task UpdateEmail(ClaimsPrincipal claimsPrincipal, string email)
    {
        var user = await GetByClaims(claimsPrincipal);
        user.Email = email;
        await _userManager.SetEmailAsync(user, email);
    }
    
    public async Task UpdatePassword(ClaimsPrincipal claimsPrincipal, string oldPassword, string newPassword)
    {
        var user = await GetByClaims(claimsPrincipal);
        await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    }
}