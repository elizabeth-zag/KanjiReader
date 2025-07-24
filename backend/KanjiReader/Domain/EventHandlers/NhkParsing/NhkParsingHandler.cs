using System.Text;
using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KanjiReader.Domain.EventHandlers.NhkParsing;

public class NhkParsingHandler(IServiceScopeFactory serviceScopeFactory) : CommonEventHandler(serviceScopeFactory)
{
    // dependencies
    private IProcessingResultRepository _processingResultRepository;
    private UserAccountService _userAccountService;
    private KanjiService _kanjiService;
    private NhkClient _nhkClient;
    private IGenerationRulesService<NhkParsingData> _generationRulesService;
    private IUserGenerationStateRepository _userGenerationStateRepository;
    private IEventRepository _eventRepository;
    private TextService _textService;
    private KanjiReaderDbContext _dbContext;
    
    protected override async Task Execute(string userId, string stringData, CancellationToken cancellationToken)
    {
        await StartProcessingTexts(userId, stringData, cancellationToken);
    }
    

    protected override async Task<(ProcessingResult[] results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount, 
        CancellationToken cancellationToken) 
    {
        NhkParsingData? previousData = null;

        var articleUrlsByDate = await _nhkClient.GetArticleUrls(cancellationToken);
        
        if (generationState != null)
        {
            previousData = JsonSerializer.Deserialize<NhkParsingData>(generationState.Data);
        }

        previousData?.SetOrderedDates(articleUrlsByDate.Keys.ToArray());

        var parsingData = _generationRulesService.GetNextState(previousData).SetOrderedDates(articleUrlsByDate.Keys.ToArray());
        generationState = generationState?.UpdateData(JsonSerializer.Serialize(parsingData)) 
                          ?? new UserGenerationState(
                              user.Id,
                              GetSourceType(),
                              JsonSerializer.Serialize(parsingData));
        
        var kanjiCharacters = (await _kanjiService.GetUserKanji(user, cancellationToken)).ToHashSet();
        
        var articleUrls = articleUrlsByDate[parsingData.CurrentDate];
        
        var suitableResult = new List<ProcessingResult>();
        if (!articleUrls.Any())
        {
            return ([], generationState); // todo: look into this
        }

        foreach (var url in articleUrls)
        {
            var resultText = await ProcessUrl(kanjiCharacters, url, cancellationToken);
            if (!string.IsNullOrEmpty(resultText))
            {
                suitableResult.Add(new ProcessingResult(
                    user.Id,
                    GetSourceType(),
                    resultText,
                    url));
                
                if (suitableResult.Count >= remainingTextCount)
                {
                    break;
                }
            }
        }
        
        return (suitableResult.ToArray(), generationState);
    }
    
    private async Task<string> ProcessUrl(HashSet<char> kanjiCharacters, string url, CancellationToken cancellationToken)
    {
        var resultString = new StringBuilder();
        var text = await _nhkClient.GetHtml(url, cancellationToken);
        
        foreach (var ch in text)
        {
            if (IsKanji(ch) && !kanjiCharacters.Contains(ch))
            {
                return string.Empty;
            }
    
            resultString.Append(ch);
        }
        
        return text;
    }
    
    private static bool IsKanji(char c)
    {
        int code = c;
        if (code >= 0x4E00 && code <= 0x9FFF)
            return true;
        if (code >= 0x3400 && code <= 0x4DBF)
            return true;
        return false;
    }

    protected override void SetScopedDependencies(IServiceScope scope)
    {
        throw new NotImplementedException();
    }

    protected override EventType GetEventType()
    {
        return EventType.NhkParsing;
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Nhk;
    }
}