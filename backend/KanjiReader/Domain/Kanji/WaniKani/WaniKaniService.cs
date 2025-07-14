using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Domain.Kanji.WaniKani;

public class WaniKaniService
{
    private readonly WaniKaniClient _waniKaniClient;
    
    public WaniKaniService(WaniKaniClient waniKaniClient)
    {
        _waniKaniClient = waniKaniClient;
    }
    
    public async Task<IReadOnlySet<char>> GetWaniKaniKanji(string token, CancellationToken cancellationToken)
    {
        // todo: NRE
        var subjectIds = await _waniKaniClient.GetAssignments(token, cancellationToken);
        return await _waniKaniClient.GetBurnedKanji(token, subjectIds, cancellationToken);
    }
}