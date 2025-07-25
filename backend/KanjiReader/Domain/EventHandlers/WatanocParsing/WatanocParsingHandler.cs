﻿using System.Text;
using System.Text.Json;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KanjiReader.Domain.EventHandlers.WatanocParsing;

public class WatanocParsingHandler(IServiceScopeFactory serviceScopeFactory) : CommonEventHandler(serviceScopeFactory)
{
    private IProcessingResultRepository _processingResultRepository;
    private UserAccountService _userAccountService;
    private KanjiService _kanjiService;
    private WatanocClient _watanocClient;
    
    
    
    // dependencies
    private IGenerationRulesService<WatanocParsingData> _generationRulesService;
    private IUserGenerationStateRepository _userGenerationStateRepository;
    private IEventRepository _eventRepository;
    private TextService _textService;
    private KanjiReaderDbContext _dbContext;
    
    protected override async Task Execute(string userId, string stringData, CancellationToken cancellationToken)
    {
        await StartProcessingTexts(userId, stringData, cancellationToken);
    }

    protected override void SetScopedDependencies(IServiceScope scope)
    {
        _processingResultRepository = scope.ServiceProvider.GetRequiredService<IProcessingResultRepository>();
        _userAccountService = scope.ServiceProvider.GetRequiredService<UserAccountService>();
        _kanjiService = scope.ServiceProvider.GetRequiredService<KanjiService>();
        _watanocClient = scope.ServiceProvider.GetRequiredService<WatanocClient>();
    }
    
    protected override EventType GetEventType()
    {
        return EventType.WatanocParsing;
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Watanoc;
    }

    protected override async Task<(ProcessingResult[] results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount, 
        CancellationToken cancellationToken)
    {
        WatanocParsingData? previousData = null;
        if (generationState != null)
        {
            previousData = JsonSerializer.Deserialize<WatanocParsingData>(generationState.Data);
        }

        var parsingData = _generationRulesService.GetNextState(previousData);
        generationState = generationState?.UpdateData(JsonSerializer.Serialize(parsingData)) 
                          ?? new UserGenerationState(
                              user.Id,
                              GetSourceType(),
                              JsonSerializer.Serialize(parsingData));
        
        var kanjiCharacters = (await _kanjiService.GetUserKanji(user, cancellationToken)).ToHashSet();
        
        var suitableResult = new List<ProcessingResult>();
        var urls = await _watanocClient.GetArticleUrls(parsingData.Category, parsingData.PageNumber, cancellationToken);
        if (!urls.Any())
        {
            return ([], generationState);
        }

        foreach (var url in urls)
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
        var text = await _watanocClient.GetHtml(url, cancellationToken);
        
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
}