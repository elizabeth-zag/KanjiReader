using System.Net.Http.Headers;
using System.Text.Json;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.ExternalServices.WaniKani.Contracts;
using Microsoft.Extensions.Options;

namespace KanjiReader.ExternalServices.WaniKani;

public class WaniKaniClient(IHttpClientFactory httpClientFactory, IOptionsMonitor<WaniKaniOptions> options)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<IReadOnlyCollection<int>> GetAssignments(
        string token, 
        IReadOnlyCollection<WaniKaniStage> stages, 
        CancellationToken cancellationToken)
    {
        var srsStages = stages.Count > 0 
            ? string.Join(",", stages.Select(ConvertWaniKaniStageToInt).SelectMany(x => x).ToArray())
            : "7,8,9";
        var url = $"https://api.wanikani.com/v2/assignments?subject_types=kanji&srs_stages={srsStages}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        AddAuthorizationHeader(token);
        
        using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);

        var response = await JsonSerializer.DeserializeAsync<ApiPage<Assignment>>(stream, cancellationToken: cancellationToken);

        if (response?.Data is null)
        {
            throw new InvalidOperationException("WaniKani returned nothing, check your token");
        }

        return response.Data.Select(d => d.Data.SubjectId).ToArray();
    }
    
    public async Task<IReadOnlySet<char>> GetMasteredKanji(string token, 
        IReadOnlyCollection<int> subjectIds, CancellationToken cancellationToken) 
    {
        var characters = new List<char>();
        
        AddAuthorizationHeader(token);

        for (int i = 0; i < subjectIds.Count; i += options.CurrentValue.BatchSize)
        {
            var batch = subjectIds.Skip(i).Take(options.CurrentValue.BatchSize);
            var url = "https://api.wanikani.com/v2/subjects?types=kanji&ids=" + string.Join(",", batch);
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
        
            using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);

            var response = await JsonSerializer.DeserializeAsync<ApiPage<Subject>>(stream, cancellationToken: cancellationToken);
            
            characters.AddRange(response?.Data?.Select(d => d.Data.Characters).ToArray() ?? []);
        }

        return characters.ToHashSet();
    }

    private void AddAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization ??= new AuthenticationHeaderValue("Bearer", token);
    }
    
    private static int[] ConvertWaniKaniStageToInt(WaniKaniStage stage)
    {
        return stage switch
        {
            WaniKaniStage.Apprentice => [1, 2, 3, 4],
            WaniKaniStage.Guru => [5, 6],
            WaniKaniStage.Master => [7],
            WaniKaniStage.Enlightened => [8],
            WaniKaniStage.Burned => [9],
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
    }
}