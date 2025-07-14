using HtmlAgilityPack;

namespace KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;

public class WatanocClient
{
    private HttpClient _httpClient;
    
    public WatanocClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string[]> GetArticleUrls(string category, int pageNumber)
    {
        var urls = new List<string>();
        var doc = new HtmlDocument();
        
        var result = await _httpClient.GetStringAsync($"https://watanoc.com/category/{category}/page/{pageNumber}");
        doc.LoadHtml(result);

        var links = doc.DocumentNode
            .SelectNodes("//a[@href]")?
            .Where(node => node.GetAttributeValue("href", "").Contains("post"));
        
        if (links != null)
        {
            foreach (var link in links)
            {
                urls.Add(link.GetAttributeValue("href", string.Empty));
            }
        }
        
        return urls.ToArray();
    }
    
    public async Task<string> GetHtml(string url)
    {
        using var client = new HttpClient();
        var result = await client.GetStringAsync(url);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(result);
        
        var className = "entry entry-content";
        var html = doc.DocumentNode.SelectSingleNode($"//div[contains(@class, '{className}')]");
        return html is null ? String.Empty : html.InnerText;
    }
}