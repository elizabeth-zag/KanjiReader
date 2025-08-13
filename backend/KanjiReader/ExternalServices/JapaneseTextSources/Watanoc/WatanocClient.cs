using HtmlAgilityPack;
using KanjiReader.Infrastructure.Repositories.Cache;

namespace KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;

public class WatanocClient(IHttpClientFactory httpClientFactory, IWatanocCacheRepository cacheRepository)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<string[]> GetArticleUrls(string category, int pageNumber, CancellationToken cancellationToken)
    {
        var cachedUrls = await cacheRepository.GetArticleUrls(category, pageNumber);
        if (cachedUrls.Any())
        {
            return cachedUrls;
        }
        
        var urls = new HashSet<string>();
        var doc = new HtmlDocument();
        
        var result = await _httpClient.GetStringAsync(
            $"https://watanoc.com/category/{category}/page/{pageNumber}", cancellationToken);
        doc.LoadHtml(result);

        var links = doc.DocumentNode
            .SelectNodes("//div[@id='content']/section/div/article/div[@class='loop-article-content']/h1/a[@href]");
        
        if (links != null)
        {
            foreach (var link in links)
            {
                urls.Add(link.GetAttributeValue("href", string.Empty));
            }
            
            await cacheRepository.SetArticleUrls(category, pageNumber, urls.ToArray());
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
        var resultString = await client.GetStringAsync(url, cancellationToken);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(resultString);
        
        var html = doc.DocumentNode.SelectSingleNode($"//div[contains(@class, 'entry entry-content')]");
        var result = html is null ? String.Empty : html.InnerText;
        
        if (!string.IsNullOrEmpty(result))
        {
            await cacheRepository.SetHtml(url, result);
        }

        return result;
    }
}