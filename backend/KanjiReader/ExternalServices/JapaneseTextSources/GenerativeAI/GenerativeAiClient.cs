using Claudia;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using Microsoft.Extensions.Options;

namespace KanjiReader.ExternalServices.JapaneseTextSources.GenerativeAI;

public class GenerativeAiClient(IOptionsMonitor<AiApiOptions> options)
{
    public async Task<AiGenerationResult> GenerateText(IReadOnlySet<char> kanji, CancellationToken cancellationToken)
    {
        var prompt = GetPrompt(string.Join(", ", kanji));
        var anthropic = new Anthropic { ApiKey = options.CurrentValue.Token };

        var response = await anthropic.Messages.CreateAsync(new()
        {
            Model = "claude-haiku-4-5-20251001",
            MaxTokens = 1024,
            
            Messages =
            [
                new() { Role = "user", Content = GetPrompt(prompt) }
            ]
        }, new RequestOptions(), cancellationToken);
        
        var inputTokens = response.Usage.InputTokens;
        var outputTokens = response.Usage.OutputTokens;
        var totalTokens = inputTokens + outputTokens;
        
        if (!AiParsingHelper.TryParseText(response.Content.ToString(), out var result, out var error) || result is null)
        {
            throw new InvalidOperationException($"Failed to deserialize response from Claude AI. Response: {response.Content}. Error: {error}");
        }
        
        return new AiGenerationResult(result.Title, result.Content, totalTokens);
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