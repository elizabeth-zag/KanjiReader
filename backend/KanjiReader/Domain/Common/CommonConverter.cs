using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.Login;
using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Domain.Common;

public static class CommonConverter
{
    public static User Convert(RegisterRequest request, DateTime loginTime)
    {
        var user = new User
        {
            UserName = request.UserName, 
            Email = request.Email,
            LastLogin = loginTime
        };

        if (string.IsNullOrEmpty(request.WaniKaniToken))
        {
            user.KanjiSourceType = KanjiSourceType.ManualSelection;
        }
        else
        {
            user.KanjiSourceType = KanjiSourceType.WaniKani;
            user.WaniKaniToken = request.WaniKaniToken;
        }

        return user;
    }
    
    public static RegisterResponse Convert(IdentityResult result)
    {
        return result.Succeeded 
            ? new RegisterResponse { StatusCode = RegistrationResultStatusCode.Success }
            : new RegisterResponse {  StatusCode = RegistrationResultStatusCode.ValidationFailure };
    }
}