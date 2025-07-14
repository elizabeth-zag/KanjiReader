using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.Login;
using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Domain.Common;

public class CommonConverter
{
    public static User Convert(RegisterRequest request, DateTime loginTime)
    {
        return new User
        {
            UserName = request.UserName, 
            Email = request.Email,
            LastLogin = loginTime
        };
    }
    
    public static RegisterResponse Convert(IdentityResult result)
    {
        return result.Succeeded 
            ? new RegisterResponse { StatusCode = RegistrationResultStatusCode.Success }
            : new RegisterResponse {  StatusCode = RegistrationResultStatusCode.ValidationFailure };
    }
    
    public static GenerationSourceType Convert(string sourceType)
    {
        return sourceType switch
        {
            "Watanoc" => GenerationSourceType.Watanoc,
            _ => GenerationSourceType.Unspecified
        };
    }
}