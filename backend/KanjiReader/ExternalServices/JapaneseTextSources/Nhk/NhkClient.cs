using System.Text.Json;
using HtmlAgilityPack;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk.Contracts;

namespace KanjiReader.ExternalServices.JapaneseTextSources.Nhk;

public class NhkClient : IHtmlParser
{
    private readonly HttpClient _httpClient;
    
    public NhkClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Dictionary<DateTime, string[]>> GetArticleUrls(CancellationToken cancellationToken)
    {
        var uri = "https://www3.nhk.or.jp/news/easy/news-list.json";
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        
        using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);

        var response = await JsonSerializer
            .DeserializeAsync<Dictionary<DateTime, NhkNewsData[]>[]>(stream, cancellationToken: cancellationToken); // todo: null
     
        return response
            .SelectMany(r => r)
            .ToDictionary(
                r => r.Key, 
                r => r.Value
                    .Select(v => $"https://www3.nhk.or.jp/news/easy/{v.NewsId}/{v.NewsId}.html")
                    .ToArray());
    }
    
    public async Task<string> ParseHtml(string url, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url, cancellationToken);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(response);
        
        var className = "article-body";
        
        return doc.DocumentNode
            .SelectNodes($"//div[contains(@class, '{className}')]/p/span")?
            .Select(GetTextFromNode)
            .OfType<string>()
            .Select(s => s.ToCharArray().First()) // todo: handle 
            .ToString() ?? "";
    }

    private string? GetTextFromNode(HtmlNode node)
    {
        var innerText = node.ChildNodes.FirstOrDefault(cn => cn.Name is "ruby" or "#text");

        if (innerText is null)
        {
            return null;
        }
            
        return innerText.HasChildNodes 
            ? innerText.ChildNodes
                .FirstOrDefault(cn => cn.Name == "#text")?
                .InnerText
            : innerText.InnerText;
    }
}