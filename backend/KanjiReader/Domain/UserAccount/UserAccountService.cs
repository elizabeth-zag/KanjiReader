using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KanjiReader.Domain.UserAccount;

public class UserAccountService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    EmailSender emailSender,
    IOptionsMonitor<UserDataOptions> userDataOptions)
{
    private User? _currentUser;

    public async Task Register(RegisterRequest dto, DateTime loginTime)
    {
        var user = CommonConverter.Convert(dto, loginTime);
        var result = await userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(Register)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await SendConfirmationCode(user, dto.Email);
    }

    public async Task<LogInResult> LogIn(LogInRequest dto, DateTime loginTime)
    {
        var user = await userManager.FindByNameAsync(dto.UserName);
        if (user == null)
            return new (null, false);

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signInResult.Succeeded && !signInResult.IsNotAllowed)
        {
            return new(null, false);
        }
        
        if (signInResult.IsNotAllowed)
        {
            var isEmailConfirmed = await ConfirmEmail(user, dto.ConfirmationCode);
            if (!isEmailConfirmed)
            {
                return new(null, true);
            }
                
        }
            
        signInResult = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signInResult.Succeeded) 
        {
            return new(null, false);
        }
        await UpdateLoginTime(user, loginTime);
        return new(user, false);
    }

    public async Task LogOut()
    {
        await signInManager.SignOutAsync();
    }

    private async Task<bool> ConfirmEmail(User user, string? code)
    {
        if (user.EmailConfirmed)
            return true;

        if (string.IsNullOrEmpty(code))
        {
            return false;
        }

        if (user.EmailConfirmCodeHash == null ||
            user.EmailConfirmExpiresAt == null ||
            user.EmailConfirmExpiresAt < DateTime.UtcNow)
            return false;

        if (user.EmailConfirmAttempts >= 5)
            return false;

        var hash = Utilities.Hash(code);

        if (!CryptographicOperations.FixedTimeEquals(
                hash, user.EmailConfirmCodeHash))
        {
            user.EmailConfirmAttempts++;
            await userManager.UpdateAsync(user);
            return false;
        }

        user.EmailConfirmed = true;
        user.EmailConfirmCodeHash = null;
        user.EmailConfirmExpiresAt = null;
        user.EmailConfirmAttempts = 0;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task SendConfirmationCode(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
            throw new KeyNotFoundException("User not found");
        if (user.Email == null)
            throw new InvalidOperationException("User has no email");
        
        await SendConfirmationCode(user, user.Email!);
    }
    
    private async Task SendConfirmationCode(User user, string email)
    {
        var code = Utilities.GenerateCode();
        user.EmailConfirmCodeHash = Utilities.Hash(code);
        user.EmailConfirmExpiresAt = DateTime.UtcNow.AddMinutes(60);
        
        await userManager.UpdateAsync(user);
        
        await emailSender.SendConfirmation(email, code);
    }
    
    public async Task<User> GetById(string userId)
    {
        _currentUser ??= _currentUser = await userManager.FindByIdAsync(userId);

        if (_currentUser == null)
        {
            throw new KeyNotFoundException($"User {userId} not found by id");
        }

        return _currentUser;
    }
    
    public async Task<User> GetByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        _currentUser ??= await userManager.GetUserAsync(claimsPrincipal);

        if (_currentUser == null)
        {
            throw new KeyNotFoundException($"User {claimsPrincipal.Identity?.Name ?? string.Empty} not found by claims principal");
        }

        return _currentUser;
    }

    private async Task UpdateLoginTime(User user, DateTime loginTime)
    {
        user.LastLogin = loginTime;
        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateLoginTime)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdateProcessingTime(User user, DateTime loginTime)
    {
        user.LastProcessingTime = loginTime;
        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateProcessingTime)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdateHasData(User user, bool hasData)
    {
        user.HasData = hasData;
        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateHasData)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdateWaniKaniToken(ClaimsPrincipal claimsPrincipal, string token)
    {
        var user = await GetByClaimsPrincipal(claimsPrincipal);
        user.WaniKaniToken = token;
        user.KanjiSourceType = KanjiSourceType.WaniKani;
        user.WaniKaniStages ??= [WaniKaniStage.Master, WaniKaniStage.Enlightened, WaniKaniStage.Burned];
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateWaniKaniToken)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdateWaniKaniStages(ClaimsPrincipal claimsPrincipal, WaniKaniStage[] stages)
    {
        var user = await GetByClaimsPrincipal(claimsPrincipal);
        user.WaniKaniStages = stages;
        user.KanjiSourceType = KanjiSourceType.WaniKani;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateWaniKaniToken)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdateThreshold(ClaimsPrincipal claimsPrincipal, double? threshold)
    {
        var user = await GetByClaimsPrincipal(claimsPrincipal);
        user.Threshold = threshold;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateThreshold)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdateKanjiSourceType(User user, KanjiSourceType kanjiSourceType)
    {
        user.KanjiSourceType = kanjiSourceType;
        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateKanjiSourceType)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdateEmail(ClaimsPrincipal claimsPrincipal, string email)
    {
        var user = await GetByClaimsPrincipal(claimsPrincipal);
        var result = await userManager.SetEmailAsync(user, email);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdateEmail)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task UpdatePassword(ClaimsPrincipal claimsPrincipal, string oldPassword, string newPassword)
    {
        var user = await GetByClaimsPrincipal(claimsPrincipal);
        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(UpdatePassword)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    
    public async Task<bool> DeleteUserAccount(ClaimsPrincipal claimsPrincipal, string password)
    {
        var user = await GetByClaimsPrincipal(claimsPrincipal);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return false;
        }
        
        var result = await userManager.DeleteAsync(user);
        
        if (!result.Succeeded)
        {
            throw new Exception($"{nameof(DeleteUserAccount)} was not successful: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        
        await signInManager.SignOutAsync();
        return true;
    }
    
    public async Task<IReadOnlyCollection<User>> GetInactiveUsers(CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Where(u => u.LastLogin < DateTime.UtcNow.AddMonths(-userDataOptions.CurrentValue.InactivityLimitMonths))
            .ToArrayAsync(cancellationToken);
    }
}