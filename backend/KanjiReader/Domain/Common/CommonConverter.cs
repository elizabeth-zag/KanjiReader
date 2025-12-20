using KanjiReader.Domain.DomainObjects;
using KanjiReader.ExternalServices.KanjiApi.Contracts;
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
            LastLogin = loginTime,
            EmailConfirmed = false,
             
        };

        if (string.IsNullOrEmpty(request.WaniKaniToken))
        {
            user.KanjiSourceType = KanjiSourceType.ManualSelection;
        }
        else
        {
            user.KanjiSourceType = KanjiSourceType.WaniKani;
            user.WaniKaniStages = [WaniKaniStage.Master, WaniKaniStage.Enlightened, WaniKaniStage.Burned];
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
    
    public static KanjiWithData Convert(Infrastructure.Database.Models.Kanji kanji)
    {
        return new KanjiWithData
        {
            Character = kanji.Character,
            KunReadings = kanji.KunReading,
            OnReadings = kanji.OnReading,
            Meanings = kanji.Meaning
        };
    }
    
    public static KanjiWithData Convert(KanjiApiDto kanji)
    {
        return new KanjiWithData
        {
            Character = kanji.Kanji,
            KunReadings = string.Join(", ", kanji.KunReadings),
            OnReadings = string.Join(", ", kanji.OnReadings),
            Meanings = string.Join(", ", kanji.Meanings)
            
        };
    }
    
    public static Infrastructure.Database.Models.Kanji Convert(KanjiWithData kanji)
    {
        return new Infrastructure.Database.Models.Kanji
        {
            Character = kanji.Character,
            KunReading = kanji.KunReadings,
            OnReading = kanji.OnReadings,
            Meaning = kanji.Meanings
            
        };
    }
}