using System.Text;
using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KanjiReader.Domain.EventHandlers.WatanocParsing;

public class WatanocParsingHandler(
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private IEventRepository _eventRepository;
    private IProcessingResultRepository _processingResultRepository;
    private UserAccountService _userAccountService;
    private KanjiService _kanjiService;
    private WatanocClient _watanocClient;
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            SetScopedDependencies(scope);
            
            var events = await _eventRepository.GetByType(EventType.WatanocParsing, cancellationToken);

            foreach (var ev in events)
            {
                var data = JsonSerializer.Deserialize<WatanocParsingData>(ev.Data); // todo: NRE
                var user = await _userAccountService.GetById(ev.UserId);
                var result = await ProcessText(user, data, cancellationToken);

                if (result.Length > 0)
                {
                    await _processingResultRepository.Insert(result, cancellationToken);
                }
            }
        }
    }

    private void SetScopedDependencies(IServiceScope scope)
    {
        _eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
        _processingResultRepository = scope.ServiceProvider.GetRequiredService<IProcessingResultRepository>();
        _userAccountService = scope.ServiceProvider.GetRequiredService<UserAccountService>();
        _kanjiService = scope.ServiceProvider.GetRequiredService<KanjiService>();
        _watanocClient = scope.ServiceProvider.GetRequiredService<WatanocClient>();
    }

    private async Task<ProcessingResult[]> ProcessText(User user, 
        WatanocParsingData data, CancellationToken cancellationToken)
    {
        var kanjiCharacters = (await _kanjiService.GetUserKanji(user, cancellationToken)).ToHashSet();
        
        var suitableResult = new List<ProcessingResult>();
        var urls = await _watanocClient.GetArticleUrls(data.Category, data.PageNumber);
        if (!urls.Any())
        {
            return [];
        }

        foreach (var url in urls)
        {
            var resultText = await ProcessUrl(kanjiCharacters, url);
            if (!string.IsNullOrEmpty(resultText))
            {
                suitableResult.Add(new ProcessingResult(
                    user.Id,
                    resultText,
                    url,
                    false));
            }
                
        }
        return suitableResult.ToArray();
    }
    
    private async Task<string> ProcessUrl(HashSet<char> kanjiCharacters, string url)
    {
        var resultString = new StringBuilder();
        var text = await _watanocClient.GetHtml(url);
        
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