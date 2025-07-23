using GenerativeAI;
using KanjiReader.Domain.Kanji.WaniKani;
using Microsoft.Extensions.Configuration;

namespace KanjiReader.ExternalServices.JapaneseTextSources.GoogleGenerativeAI;

public class GoogleGenerativeAiClient
{
    private readonly IConfiguration _config;

    public GoogleGenerativeAiClient(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> GenerateAnswer(IReadOnlySet<char> kanji, CancellationToken cancellationToken)
    {
        var prompt = GetPrompt(string.Join(", ", kanji));
        var apiKey = _config["GoogleAiApiToken"]; // todo: config
        var googleAi = new GoogleAi(apiKey);

        var googleModel = googleAi.CreateGenerativeModel("models/gemini-2.5-pro");
        
        var googleResponse = await googleModel.GenerateContentAsync(prompt, cancellationToken);
        return googleResponse.Text;
    }
    
    private string GetPrompt(string kanjiText) => "Generate a short coherent text (a couple of paragraphs long) " +
                                                  $"including only kanji from this list: {kanjiText} or kana. " +
                                                  "The text should not include kanji that are not in the provided list";
}