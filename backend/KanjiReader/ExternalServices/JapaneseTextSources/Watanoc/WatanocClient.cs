using HtmlAgilityPack;

namespace KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;

public class WatanocClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<string[]> GetArticleUrls(string category, int pageNumber, CancellationToken cancellationToken)
    {
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
        }
        
        return urls.ToArray();
    }
    
    public async Task<string> ParseHtml(string url, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var result = await client.GetStringAsync(url, cancellationToken);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(result);
        
        var className = "entry entry-content";
        var html = doc.DocumentNode.SelectSingleNode($"//div[contains(@class, '{className}')]");
        return html is null ? String.Empty : html.InnerText;
    }
}