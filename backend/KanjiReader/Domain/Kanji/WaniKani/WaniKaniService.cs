using KanjiReader.Domain.DomainObjects;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories.Cache;

namespace KanjiReader.Domain.Kanji.WaniKani;

public class WaniKaniService(WaniKaniClient waniKaniClient, IKanjiCacheRepository kanjiCacheRepository)
{
    public async Task FillWaniKaniKanjiCache(User user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.WaniKaniToken))
        {
            return;
        }
        
        var userKanji = await GetWaniKaniKanji(user.WaniKaniToken, user.WaniKaniStages ?? [], cancellationToken);
        await kanjiCacheRepository.SetUserKanji(user.Id, userKanji);
    }
    
    public async Task<IReadOnlySet<char>> GetWaniKaniKanji(
        string token,
        IReadOnlyCollection<WaniKaniStage> stages,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentNullException(nameof(token));
        }
        
        var subjectIds = await waniKaniClient.GetAssignments(token, stages, cancellationToken);
        return await waniKaniClient.GetMasteredKanji(token, subjectIds, cancellationToken);
    }
}