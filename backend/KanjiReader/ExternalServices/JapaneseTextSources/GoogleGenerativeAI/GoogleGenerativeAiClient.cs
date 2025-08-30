using GenerativeAI;
using KanjiReader.Domain.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace KanjiReader.ExternalServices.JapaneseTextSources.GoogleGenerativeAI;

public class GoogleGenerativeAiClient(IOptionsMonitor<GoogleAiApiOptions> options)
{
    public async Task<(string, string)> GenerateText(IReadOnlySet<char> kanji, CancellationToken cancellationToken)
    {
        var prompt = GetPrompt(string.Join(", ", kanji));
        var googleAi = new GoogleAi(options.CurrentValue.Token);

        var googleModel = googleAi.CreateGenerativeModel("models/gemini-2.5-pro");
        var googleResponse = await googleModel.GenerateContentAsync(prompt, cancellationToken);
            
        if (!AiParsingHelper.TryParseText(googleResponse.Text, out var result, out var error) || result is null)
        {
            throw new InvalidOperationException($"Failed to deserialize response from Google AI. Response: {googleResponse.Text}. Error: {error}");
        }
            
        return (result.Title, result.Content);
    }
    
    private string GetPrompt(string kanjiText) => "You are generating a short, easy-to-read Japanese text for language learners using only the provided kanji.\n" + 
                                                  $"Kanji to use: {kanjiText}\n" +
                                                  "Requirements:\n" +
                                                  "- \"title\": short, catchy, relevant to the text, maximum 15 Japanese characters.\n" +
                                                  "- \"content\": 2–3 short paragraphs, must include only the provided kanji and kana (no other kanji).\n" +
                                                  "- Prioritize using more difficult kanji from the list.\n" +
                                                  "- Do not include romaji, furigana, translations, or explanations.\n" + "\n" + 
                                                  "Output format: Output only valid JSON. Do not include code fences, markdown formatting, or any extra text. " +
                                                  "The JSON must be in this exact format:\n" + "{\"title\":\"Your title here\",\"content\":\"Your text here\"}\n" +
                                                  "\n" + "Respond with nothing else except this JSON object.";
}