using System.Security.Claims;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.Kanji.WaniKani;

public class WaniKaniService
{
    private readonly WaniKaniClient _waniKaniClient;
    private readonly UserAccountService _userAccountService;
    private readonly IKanjiCacheRepository _kanjiCacheRepository;
    
    public WaniKaniService(WaniKaniClient waniKaniClient, UserAccountService userAccountService, IKanjiCacheRepository kanjiCacheRepository)
    {
        _waniKaniClient = waniKaniClient;
        _userAccountService = userAccountService;
        _kanjiCacheRepository = kanjiCacheRepository;
    }

    public async Task FillWaniKaniKanjiCache(ClaimsPrincipal claimsPrincipal, string token,
        CancellationToken cancellationToken)
    {
        var userKanji = await GetWaniKaniKanji(token, cancellationToken);
        var user = await _userAccountService.GetByClaims(claimsPrincipal);
        await _kanjiCacheRepository.SetUserKanji(user.Id, userKanji);
    }
    
    public async Task<IReadOnlySet<char>> GetWaniKaniKanji(string token, CancellationToken cancellationToken)
    {
        // todo: NRE
        var subjectIds = await _waniKaniClient.GetAssignments(token, cancellationToken);
        return await _waniKaniClient.GetBurnedKanji(token, subjectIds, cancellationToken);
    }
}