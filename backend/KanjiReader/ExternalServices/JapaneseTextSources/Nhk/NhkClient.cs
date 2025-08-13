using System.Text.Json;
using HtmlAgilityPack;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk.Contracts;
using KanjiReader.Infrastructure.Repositories.Cache;

namespace KanjiReader.ExternalServices.JapaneseTextSources.Nhk;

public class NhkClient(IHttpClientFactory httpClientFactory, INhkCacheRepository cacheRepository)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<Dictionary<DateTime, string[]>> GetArticleUrls(CancellationToken cancellationToken)
    {
        var cachedUrls = await cacheRepository.GetArticleUrls();
        if (cachedUrls.Any())
        {
            return cachedUrls;
        }
        
        const string uri = "https://www3.nhk.or.jp/news/easy/news-list.json";
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        
        using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);

        var response = await JsonSerializer
            .DeserializeAsync<Dictionary<DateTime, NhkNewsData[]>[]>(stream, cancellationToken: cancellationToken); // todo: null
     
        var result = response?
            .SelectMany(r => r)
            .ToDictionary(
                r => r.Key, 
                r => r.Value
                    .Select(v => $"https://www3.nhk.or.jp/news/easy/{v.NewsId}/{v.NewsId}.html")
                    .ToArray());
        if (result != null && result.Any())
        {
            await cacheRepository.SetArticleUrls(result);
        }
        return result ?? new Dictionary<DateTime, string[]>();
    }
    
    public async Task<string> ParseHtml(string url, CancellationToken cancellationToken)
    {
        var cachedHtml = await cacheRepository.GetHtml(url);
        if (!string.IsNullOrEmpty(cachedHtml))
        {
            return cachedHtml;
        }
        
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url, cancellationToken);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(response);
        
        var className = "article-body";
        
        var textParts = doc.DocumentNode
            .SelectNodes($"//div[contains(@class, '{className}')]/p/span")?
            .Select(GetTextFromNode)
            .OfType<string>();
        
        var result = textParts == null ? string.Empty : string.Join("", textParts);
        if (!string.IsNullOrEmpty(result))
        {
            await cacheRepository.SetHtml(url, result);
        }

        return result;
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