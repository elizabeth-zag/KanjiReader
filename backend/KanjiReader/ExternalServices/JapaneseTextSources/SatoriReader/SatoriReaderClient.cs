using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader.Contracts;
using KanjiReader.Infrastructure.Repositories.Cache;

namespace KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader;

public class SatoriReaderClient(IHttpClientFactory httpClientFactory, ISatoriReaderCacheRepository cacheRepository)
{
    private const string PrefixUrl = "https://www.satorireader.com";
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    
    public async Task<string[]> GetSeriesUrls(CancellationToken cancellationToken)
    {
        var cachedUrls = await cacheRepository.GetSeriesUrls();
        if (cachedUrls.Any())
        {
            return cachedUrls;
        }
        
        var doc = new HtmlDocument();

        var resultString = await _httpClient.GetStringAsync($"{PrefixUrl}/series", cancellationToken);
        doc.LoadHtml(resultString);

        var result = doc.DocumentNode
            .SelectNodes("//a[@href]")?
            .Where(node => node.GetAttributeValue("href", "").Contains("series/"))
            .Select(node => $"{PrefixUrl}{node.GetAttributeValue("href", string.Empty)}")
            .ToArray() ?? [];
        
        if (result.Any())
        {
            await cacheRepository.SetSeriesUrls(result);
        }
        
        return result ?? [];
    }
    
    public async Task<string[]> GetArticleUrls(string[] seriesUrls, CancellationToken cancellationToken) 
    { 
        var urls = new List<string>();
        foreach (var seriesUrl in seriesUrls) // todo: maybe parallelize this
        {
            var freeArticlesUrls = await GetFreeArticlesUrl(seriesUrl, cancellationToken);
            urls.AddRange(freeArticlesUrls);
        }
        
        return urls.ToArray();
    }
    
    public async Task<string> ParseHtml(string url, CancellationToken cancellationToken)
    {
        var cachedHtml = await cacheRepository.GetHtml(url);
        if (!string.IsNullOrEmpty(cachedHtml))
        {
            return cachedHtml;
        }
        
        using var client = new HttpClient();
        var html = await client.GetStringAsync(url, cancellationToken);
        
        var match = Regex.Match(html, @"var content = (\{.*?\});", RegexOptions.Singleline);
        var articleResponse = JsonSerializer.Deserialize<SatoriArticleResponse>(match.Groups[1].Value);

        if (articleResponse == null)
        {
            return string.Empty;
        }

        var resultText = new StringBuilder();

        foreach (var paragraph in articleResponse.Paragraphs)
        {
            foreach (var sentence in paragraph.Sentences)
            {
                foreach (var run in sentence.Runs)
                {
                    foreach (var part in run.Parts)
                    {
                        if (!string.IsNullOrEmpty(part.Text))
                        {
                            resultText.Append(part.Text);
                        }
                        else
                        {
                            foreach (var text in part.SatoriTexts)
                            {
                                if (!string.IsNullOrEmpty(text.Text))
                                {
                                    resultText.Append(text.Text);
                                }
                            }   
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(resultText.ToString()))
        {
            await cacheRepository.SetHtml(url, resultText.ToString());
        }
        
        return resultText.ToString();
    }

    private async Task<string[]> GetFreeArticlesUrl(string url, CancellationToken cancellationToken)
    {
        var cachedUrls = await cacheRepository.GetArticleUrls(url);
        if (cachedUrls.Any())
        {
            return cachedUrls;
        }
        
        var doc = new HtmlDocument();
        
        var resultString = await _httpClient.GetStringAsync(url, cancellationToken);
        doc.LoadHtml(resultString);

        var className = "series-detail-grid-right";
        var result = doc.DocumentNode
            .SelectNodes($"//div[contains(@class, '{className}')]/div[contains(@class, 'title')]/a[@href]")?
            .Select(n => $"{PrefixUrl}{n.GetAttributeValue("href", string.Empty)}")
            .Take(2)
            .ToArray() ?? [];

        if (result.Any())
        {
            await cacheRepository.SetArticleUrls(url, result);
        }

        return result;
    }
}