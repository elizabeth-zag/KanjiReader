using System.Text;
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
    
    public async Task<(string, string)> ParseHtml(string url, CancellationToken cancellationToken)
    {
        var cachedHtmlTitle = await cacheRepository.GetHtmlTitle(url);
        var cachedHtml = await cacheRepository.GetHtml(url);
        
        if (!string.IsNullOrEmpty(cachedHtml) && !string.IsNullOrEmpty(cachedHtmlTitle))
        {
            return (cachedHtmlTitle, cachedHtml);
        }
        
        var resultString = await httpClientFactory.CreateClient().GetStringAsync(url, cancellationToken);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(resultString);

        var potentialContent = new StringBuilder();
        
        var htmlTitle = doc.DocumentNode.SelectSingleNode($"//h1[contains(@class, 'entry-title single-title')]");
        var htmlContent = doc.DocumentNode.SelectSingleNode($"//div[contains(@class, 'entry entry-content')]");
        
        foreach (var contentChild in htmlContent?.ChildNodes ?? Enumerable.Empty<HtmlNode>())
        {
            var innerText = contentChild.InnerText.Trim();
            
            if (innerText.Length > 0)
            {
                potentialContent.Append(innerText);
                potentialContent.AppendLine();
                potentialContent.AppendLine();
            }
        }
        
        var title = htmlTitle is null ? String.Empty : htmlTitle.InnerText.Trim();
        var content = potentialContent.ToString();
        
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(content))
        {
            await cacheRepository.SetHtmlTitle(url, title);
            await cacheRepository.SetHtml(url, content);
        }

        return (title, content);
    }
}