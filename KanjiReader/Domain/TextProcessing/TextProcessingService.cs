using System.Text;
using KanjiReader.Domain.TextProcessing.DomainObjects;
using KanjiReader.Domain.WaniKani;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;

namespace KanjiReader.Domain.TextProcessing;

public class TextProcessingService
{
    private readonly WaniKaniService _waniKaniService;
    private readonly WatanocClient _watanocClient;
    private HashSet<char> KanjiCharacters = new ();
    
    public TextProcessingService(WaniKaniService waniKaniService, WatanocClient watanocClient)
    {
        _waniKaniService = waniKaniService;
        _watanocClient = watanocClient;
    }
    
    public async Task<TextProcessingResult[]> ProcessText()
    {
        KanjiCharacters = (await _waniKaniService.GetWaniKaniKanji()).ToHashSet();
        
        var pageNumber = 1;
        var suitableResult = new List<TextProcessingResult>();
        while (true)
        {
            var urls = await _watanocClient.GetArticleUrls(pageNumber);
            if (!urls.Any())
            {
                break;
            }

            foreach (var url in urls)
            {
                var result = await ProcessUrl(url);
                if (result.IsSuccess)
                {
                    suitableResult.Add(result);
                }
                
            }
            
            pageNumber++;
        }
        return suitableResult.ToArray();
    }
    
    private async Task<TextProcessingResult> ProcessUrl(string url)
    {
        var resultString = new StringBuilder();
        var text = await _watanocClient.GetHtml(url);
        
        foreach (var ch in text)
        {
            if (IsKanji(ch) && !KanjiCharacters.Contains(ch))
            {
                return TextProcessingResult.CreateFailResult();
            }
    
            resultString.Append(ch);
        }
        
        
        return TextProcessingResult.CreateSuccessResult(url, text);
    }

    private static bool IsKanji(char c)
    {
        int code = c;
        // CJK Unified Ideographs
        if (code >= 0x4E00 && code <= 0x9FFF)
            return true;
        // CJK Unified Ideographs Extension A
        if (code >= 0x3400 && code <= 0x4DBF)
            return true;
        // Add more extensions if you want (rare)
        return false;
    }
}