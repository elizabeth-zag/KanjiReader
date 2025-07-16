using System.Text.Json;
using KanjiReader.Domain.DomainObjects.KanjiLists;

namespace KanjiReader.ExternalServices.KanjiApi;

public class KanjiApiClient
{
    private readonly HttpClient _httpClient;
    
    public KanjiApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task<IReadOnlySet<char>> GetKanjiList(IReadOnlyCollection<KanjiListType> kanjiListTypes, 
        CancellationToken cancellationToken)
    {
        Task<IReadOnlySet<char>>[] tasks = kanjiListTypes
            .Select(t => GetKanjiList(t, cancellationToken))
            .ToArray();

        var results = await Task.WhenAll(tasks);
    
        return results.SelectMany(r => r).ToHashSet();
    }

    private async Task<IReadOnlySet<char>> GetKanjiList(KanjiListType kanjiListType, CancellationToken cancellationToken)
    {
        var url = ConvertKanjiListToUrl(kanjiListType);
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://kanjiapi.dev/v1/kanji/{url}");
        
        using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
    
        var response = await JsonSerializer.DeserializeAsync<char[]>(stream, cancellationToken: cancellationToken);
        
        // todo: exception handling 
    
        return response.ToHashSet();
    }

    private string ConvertKanjiListToUrl(KanjiListType kanjiListType)
    {
        return kanjiListType switch
        {
            KanjiListType.Grade1 => "grade-1",
            KanjiListType.Grade2 => "grade-2",
            KanjiListType.Grade3 => "grade-3",
            KanjiListType.Grade4 => "grade-4",
            KanjiListType.Grade5 => "grade-5",
            KanjiListType.Grade6 => "grade-6",
            KanjiListType.Grade8 => "grade-8",
            KanjiListType.JlptN5 => "jlpt-5",
            KanjiListType.JlptN4 => "jlpt-4",
            KanjiListType.JlptN3 => "jlpt-3",
            KanjiListType.JlptN2 => "jlpt-2",
            KanjiListType.JlptN1 => "jlpt-1",
            KanjiListType.Kyouiku => "kyouiku",
            KanjiListType.Heisig => "heisig",
            _ => throw new ArgumentOutOfRangeException(nameof(kanjiListType), kanjiListType, null) // todo: handle this case properly
        };
    }
}