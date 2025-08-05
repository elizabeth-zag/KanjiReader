using System.Net.Http.Headers;
using System.Text.Json;
using KanjiReader.ExternalServices.WaniKani.Contracts;

namespace KanjiReader.ExternalServices.WaniKani;

public class WaniKaniClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<IReadOnlyCollection<int>> GetAssignments(string token, CancellationToken cancellationToken)
    {
        var url = "https://api.wanikani.com/v2/assignments?subject_types=kanji&srs_stages=7,8,9";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        AddAuthorizationHeader(token);
        
        using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);

        var response = await JsonSerializer.DeserializeAsync<ApiPage<Assignment>>(stream, cancellationToken: cancellationToken);
        
        // todo: exception handling 

        return response.Data.Select(d => d.Data.SubjectId).ToArray();
    }
    
    public async Task<IReadOnlySet<char>> GetMasteredKanji(string token, 
        IReadOnlyCollection<int> subjectIds, CancellationToken cancellationToken) 
    {
        var batchSize = 100; // todo: move to options
        var characters = new List<char>();
        
        AddAuthorizationHeader(token);

        for (int i = 0; i < subjectIds.Count; i += batchSize)
        {
            var batch = subjectIds.Skip(i).Take(batchSize);
            var url = "https://api.wanikani.com/v2/subjects?types=kanji&ids=" + string.Join(",", batch);
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
        
            using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);

            var response = await JsonSerializer.DeserializeAsync<ApiPage<Subject>>(stream, cancellationToken: cancellationToken);
            // todo: exception handling 
            characters.AddRange(response.Data.Select(d => d.Data.Characters).ToArray());
        }

        return characters.ToHashSet();
    }

    private void AddAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization ??= new AuthenticationHeaderValue("Bearer", token);
    }
}