using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Kanji;
using KanjiReader.Infrastructure.Database.Models;
using Microsoft.Extensions.Logging;

namespace KanjiReader.Domain.TextProcessing;

public class TextParsingService(KanjiService kanjiService, TextService textService, ILogger<TextParsingService> logger)
{
    public async Task<IReadOnlyCollection<ProcessingResult>> ParseAndValidateText(
        User user,
        GenerationSourceType sourceType,
        int remainingTextCount,
        string[] articleUrls,
        Func<string, CancellationToken, Task<(string, string)>> parseHtmlFunction,
        CancellationToken cancellationToken)
    {
        if (articleUrls.Length == 0)
        {
            logger.LogWarning("No article urls were found for {SourceType}, user {UserId}", nameof(sourceType), user.Id);

            return [];
        }
        
        var kanjiCharacters = (await kanjiService.GetUserKanjiCharacters(user, cancellationToken)).ToHashSet();
        
        var threshold = await textService.GetThreshold(user, cancellationToken, kanjiCharacters.Count);
        
        var suitableResult = new List<ProcessingResult>();
        foreach (var url in articleUrls)
        {
            var (title, text) = await parseHtmlFunction.Invoke(url, cancellationToken);
            ValidateText(kanjiCharacters, title, text, out var ratio, out var unknownKanji);
    
            if (string.IsNullOrEmpty(text) || ratio > threshold) continue;
            
            suitableResult.Add(new ProcessingResult(
                user.Id,
                sourceType,
                title,
                text,
                url,
                ratio,
                unknownKanji.ToArray(),
                DateTime.UtcNow));
                
            if (suitableResult.Count >= remainingTextCount)
            {
                break;
            }
        }
        
        return suitableResult.ToArray();
    }
    
    public void ValidateText(
        HashSet<char> userKanji, 
        string title, 
        string text, 
        out double ratio, 
        out HashSet<char> unknownKanji)
    {
        var allKanji = new HashSet<char>();
        unknownKanji = new HashSet<char>();
        
        foreach (var ch in title.Concat(text))
        {
            if (!IsKanji(ch)) continue;
            if (!userKanji.Contains(ch))
            {
                unknownKanji.Add(ch);
            }

            allKanji.Add(ch);
        }
 
        ratio = Math.Round((double)unknownKanji.Count / allKanji.Count, 2);
        if (double.IsNaN(ratio)) ratio = 0;
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