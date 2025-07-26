using System.Text;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Kanji;
using KanjiReader.ExternalServices;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Domain.Text;

public class TextProcessingService
{
    private readonly KanjiService _kanjiService;

    public TextProcessingService(KanjiService kanjiService)
    {
        _kanjiService = kanjiService;
    }

    public async Task<IReadOnlyCollection<ProcessingResult>> ProcessText(
        User user,
        GenerationSourceType sourceType,
        int remainingTextCount,
        string[] articleUrls,
        Func<string, CancellationToken, Task<string>> func,
        CancellationToken cancellationToken)
    {
        var kanjiCharacters = (await _kanjiService.GetUserKanji(user, cancellationToken)).ToHashSet();
        var suitableResult = new List<ProcessingResult>();
        foreach (var url in articleUrls)
        {
            var rawText = await func.Invoke(url, cancellationToken);
            var resultText = ProcessUrl(kanjiCharacters, rawText);
    
            if (string.IsNullOrEmpty(resultText)) continue;
            
            suitableResult.Add(new ProcessingResult(user.Id, sourceType, resultText, url));
                
            if (suitableResult.Count >= remainingTextCount)
            {
                break;
            }
        }
        
        return suitableResult.ToArray();
    }
    
    private static string ProcessUrl(HashSet<char> kanjiCharacters, string text)
    {
        var resultString = new StringBuilder();
        
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