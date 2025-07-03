using KanjiReader.ExternalServices.WaniKani;

namespace KanjiReader.Domain.WaniKani;

public class WaniKaniService
{
    private WaniKaniClient _waniKaniClient;
    
    public WaniKaniService(WaniKaniClient waniKaniClient)
    {
        _waniKaniClient = waniKaniClient;
    }
    
    public async Task<char[]> GetWaniKaniKanji()
    {
        var subjectIds = await _waniKaniClient.GetAssignments();
        return await _waniKaniClient.GetBurnedKanji(subjectIds);
    }
}