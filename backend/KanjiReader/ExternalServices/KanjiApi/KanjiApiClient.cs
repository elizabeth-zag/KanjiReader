using System.Text.Json;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.ExternalServices.KanjiApi.Contracts;
using Microsoft.Extensions.Options;

namespace KanjiReader.ExternalServices.KanjiApi;

public class KanjiApiClient(IHttpClientFactory httpClientFactory, IOptionsMonitor<KanjiApiOptions> options)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<IReadOnlySet<char>> GetKanjiList(IReadOnlyCollection<KanjiListType> kanjiListTypes, 
        CancellationToken cancellationToken)
    {
        Task<IReadOnlySet<char>>[] tasks = kanjiListTypes
            .Where(t => t != KanjiListType.Unspecified)
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
    
        var response = await JsonSerializer.DeserializeAsync<string[]>(stream, cancellationToken: cancellationToken);
    
        return response?.Where(r => r.Length == 1).Select(char.Parse).ToHashSet() ?? [];
    }

    public async Task<IReadOnlyCollection<KanjiWithData>> GetKanjiData(IEnumerable<char> kanji, CancellationToken ct = default)
    {
        using var sem = new SemaphoreSlim(options.CurrentValue.MaxConcurrency);
        var tasks = kanji.Select(async kanjiChar =>
        {
            await sem.WaitAsync(ct);
            try
            {
                var resp = await GetKanjiData(kanjiChar, ct);
                return resp;
            }
            finally
            {
                sem.Release();
            }
        });

        var result = await Task.WhenAll(tasks);
        return result
            .Where(k => k != null)
            .Select(CommonConverter.Convert!)
            .ToArray();
    }
    private async Task<KanjiApiDto?> GetKanjiData(char character, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://kanjiapi.dev/v1/kanji/{character}");
        
        using var responseMessage = await _httpClient.SendAsync(request, cancellationToken);
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
    
        return await JsonSerializer.DeserializeAsync<KanjiApiDto>(stream, cancellationToken: cancellationToken);
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
            KanjiListType.Jouyou => "jouyou",
            KanjiListType.Heisig => "heisig",
            _ => string.Empty
        };
    }
}