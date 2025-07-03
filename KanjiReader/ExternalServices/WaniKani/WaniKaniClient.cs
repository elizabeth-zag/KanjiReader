using System.Text.Json;
using KanjiReader.ExternalServices.WaniKani.Contracts;

namespace KanjiReader.ExternalServices.WaniKani;

public class WaniKaniClient
{
    private HttpClient _httpClient;
    
    public WaniKaniClient(IHttpClientFactory httpClientFactory)
    {
        // todo: authorization token move to a secure storage 
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task<int[]> GetAssignments()
    {
        var url = "https://api.wanikani.com/v2/assignments?subject_types=kanji&burned=true";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        using var responseMessage = await _httpClient.SendAsync(request);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync();

        var response = await JsonSerializer.DeserializeAsync<ApiPage<Assignment>>(stream);
        
        // todo: exception handling 

        return response.Data.Select(d => d.Data.SubjectId).ToArray();
    }
    
    public async Task<char[]> GetBurnedKanji(int[] subjectIds)
    {
        var batchSize = 100; // todo: move to options
        var characters = new List<char>();

        for (int i = 0; i < subjectIds.Length; i += batchSize)
        {
            var batch = subjectIds.Skip(i).Take(batchSize);
            var url = "https://api.wanikani.com/v2/subjects?types=kanji&ids=" + string.Join(",", batch);
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
        
            using var responseMessage = await _httpClient.SendAsync(request);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync();

            var response = await JsonSerializer.DeserializeAsync<ApiPage<Subject>>(stream);
            // todo: exception handling 
            characters.AddRange(response.Data.Select(d => d.Data.Characters).ToArray());
        }

        return characters.ToArray();
    }
}