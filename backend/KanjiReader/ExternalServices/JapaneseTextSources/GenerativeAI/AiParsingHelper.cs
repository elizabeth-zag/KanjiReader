using System.Text.Json;
using System.Text.RegularExpressions;
using KanjiReader.ExternalServices.JapaneseTextSources.GenerativeAI.Contracts;

namespace KanjiReader.ExternalServices.JapaneseTextSources.GenerativeAI;

public static class AiParsingHelper
{
    public static bool TryParseText(string raw, out Text? text, out string? error)
    {
        text = null;
        error = null;

        var fenced = TryExtractFencedJson(raw);
        var candidate = fenced ?? TryExtractBalancedJson(raw);
        if (candidate is null)
        {
            error = "No JSON found in model response.";
            return false;
        }
        
        candidate = Clean(candidate);

        try
        {
            using var _ = JsonDocument.Parse(candidate);
            text = JsonSerializer.Deserialize<Text>(candidate);
            if (text is null) { error = "Deserialization returned null."; return false; }
            return true;
        }
        catch (Exception ex)
        {
            error = $"JSON parse/deserialize failed: {ex.Message}\nExtracted: {candidate}";
            return false;
        }
    }

    static string? TryExtractFencedJson(string s)
    {
        var rx = new Regex(@"```(?:json|JSON)?\s*(\{[\s\S]*?\})\s*```",
                           RegexOptions.Singleline | RegexOptions.Compiled);
        var m = rx.Match(s);
        return m.Success ? m.Groups[1].Value : null;
    }
    static string? TryExtractBalancedJson(string s)
    {
        int i = s.IndexOf('{');
        if (i < 0) return null;

        bool inStr = false;
        bool esc = false;
        int depth = 0;

        for (int j = i; j < s.Length; j++)
        {
            char c = s[j];

            if (inStr)
            {
                if (esc) esc = false;
                else if (c == '\\') esc = true;
                else if (c == '\"') inStr = false;
            }
            else
            {
                if (c == '\"') inStr = true;
                else if (c == '{') depth++;
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                        return s.Substring(i, j - i + 1);
                }
            }
        }
        return null;
    }

    static string Clean(string json)
    {
        return json.Trim()
                   .Replace('“', '"')
                   .Replace('”', '"')
                   .Replace('’', '\'')
                   .Replace("\uFEFF", "");
    }
}
