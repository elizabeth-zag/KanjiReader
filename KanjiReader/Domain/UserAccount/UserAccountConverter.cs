using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.LogIn;
using KanjiReader.Presentation.Dtos.Register;
using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Domain.UserAccount;

public class UserAccountConverter
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
    
    public static LogInResponse Convert(LogInResultStatusCode statusCode, string jwtToken)
    {
        return new LogInResponse {  StatusCode = statusCode };
    }
}