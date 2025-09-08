﻿using KanjiReader.Domain.Deletion;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.Login;
using KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController(
    UserAccountService userAccountService,
    TextService textService,
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
        await userAccountService.LogOut();
        return Ok();
    }
    
    [Authorize]
    [HttpGet(nameof(GetCurrentUser))]
    public GetCurrentUserResponse GetCurrentUser()
    {
        return new GetCurrentUserResponse
        {
            UserName = User.Identity?.Name ?? string.Empty,
        };
    }
    
    [Authorize]
    [HttpPost(nameof(SetWaniKaniToken))]
    public async Task SetWaniKaniToken(SetWaniKaniTokenRequest dto, CancellationToken cancellationToken)
    {
        await userAccountService.UpdateWaniKaniToken(User, dto.Token);
        var user = await userAccountService.GetByClaimsPrincipal(User);

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
    [HttpPost(nameof(SetWaniKaniStages))]
    public async Task SetWaniKaniStages(SetWaniKaniStagesRequest dto, CancellationToken cancellationToken)
    {
        await userAccountService.UpdateWaniKaniStages(User, dto.Stages);
        var user = await userAccountService.GetByClaimsPrincipal(User);

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
    [HttpGet(nameof(GetWaniKaniStages))]
    public async Task<GetWaniKaniStagesResponse> GetWaniKaniStages()
    {
        var user = await userAccountService.GetByClaimsPrincipal(User);

        return new GetWaniKaniStagesResponse { Stages = user.WaniKaniStages ?? [] };
    }
    
    [Authorize]
    [HttpPost(nameof(SetUserThreshold))]
    public async Task SetUserThreshold(SetUserThresholdRequest dto)
    {
        await userAccountService.UpdateThreshold(User, dto.Threshold);
    }

    [HttpGet(nameof(GetUserThreshold))]
    public async Task<GetUserThresholdResponse> GetUserThreshold(CancellationToken cancellationToken)
    {
        var (threshold, isUserSet) = await textService.GetThreshold(User, cancellationToken);
        
        return new GetUserThresholdResponse
        {
            Threshold = threshold,
            IsUserSet = isUserSet
        };
    }
    
    [Authorize]
    [HttpGet(nameof(GetEmail))]
    public async Task<GetEmailResponse> GetEmail()
    {
        var user = await userAccountService.GetByClaimsPrincipal(User);
        
        return new  GetEmailResponse { Email = user.Email ?? string.Empty };
    }
    
    [Authorize]
    [HttpPost(nameof(UpdateEmail))]
    public async Task<IActionResult> UpdateEmail(UpdateEmailRequest dto)
    {
        if (dto.NeedDelete)
        {
            await userAccountService.DeleteEmail(User);
        }
        else
        {
            await userAccountService.UpdateEmail(User, dto.Email);
        }
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(UpdatePassword))]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest dto)
    {
        await userAccountService.UpdatePassword(User, dto.OldPassword, dto.NewPassword);
        return Ok();
    }
    
    [Authorize]
    [HttpPost(nameof(DeleteUserAccount))]
    public async Task<IActionResult> DeleteUserAccount(DeleteUserAccountRequest dto, CancellationToken cancellationToken)
    {
        var isDeletionSuccessful = await deletionService.DeleteUser(User, dto.Password, cancellationToken);
        if (!isDeletionSuccessful)
        {
            return Unauthorized(new { message = "Invalid password" });
        }
        
        return Ok();
    }
}