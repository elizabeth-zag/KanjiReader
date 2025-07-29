using System.Text;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Kanji;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Domain.TextProcessing;

public class TextParsingService
{
    private readonly KanjiService _kanjiService;

    public TextParsingService(KanjiService kanjiService)
    {
        _kanjiService = kanjiService;
    }

    public async Task<IReadOnlyCollection<ProcessingResult>> ParseAndValidateText(
        User user,
        GenerationSourceType sourceType,
        int remainingTextCount,
        string[] articleUrls,
        Func<string, CancellationToken, Task<string>> parseHtmlFunction,
        CancellationToken cancellationToken)
    {
        var kanjiCharacters = (await _kanjiService.GetUserKanji(user, cancellationToken)).ToHashSet();
        
        var suitableResult = new List<ProcessingResult>();
        foreach (var url in articleUrls)
        {
            var rawText = await parseHtmlFunction.Invoke(url, cancellationToken);
            var resultText = ValidateText(kanjiCharacters, rawText, out var ratio);
    
            if (string.IsNullOrEmpty(resultText)) continue;
            
            suitableResult.Add(new ProcessingResult(user.Id, sourceType, resultText, url, ratio));
                
            if (suitableResult.Count >= remainingTextCount)
            {
                break;
            }
        }
        var orderedResults = suitableResult
            .OrderBy(r => r.UnknownKanji)
            .ToList();
        
        return suitableResult.ToArray();
    }
    
    private string ValidateText(HashSet<char> knownKanji, string text, out double ratio)
    {
        var resultString = new StringBuilder();
        var unknownKanji = new HashSet<char>();
        
        foreach (var ch in text)
        {
            if (IsKanji(ch) && !knownKanji.Contains(ch))
            {
                unknownKanji.Add(ch);
            }
    
            resultString.Append(ch);
        }
 
        ratio = Math.Round((double)unknownKanji.Count / knownKanji.Count, 2);
        
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