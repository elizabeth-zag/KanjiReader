using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.Login;
using KanjiReader.Presentation.Dtos.Texts;

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
    
    public static ProcessingResultDto Convert(ProcessingResult result)
    {
        return new ProcessingResultDto
        {
            Id = result.Id,
            Title = result.Title,
            Content = result.Text,
            Url = result.Url,
            SourceType = result.SourceType,
            Ratio = result.UnknownKanjiRatio,
            UnknownKanji = result.UnknownKanji.ToArray(),
            CreateDate = result.CreateDate
        };
    }
}