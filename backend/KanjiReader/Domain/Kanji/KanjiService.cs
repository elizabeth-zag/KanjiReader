using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.Kanji;

public class KanjiService
{
    private readonly WaniKaniService _waniKaniService;
    private readonly IKanjiRepository _kanjiRepository;

    public KanjiService(WaniKaniService waniKaniService, IKanjiRepository kanjiRepository)
    {
        _waniKaniService = waniKaniService;
        _kanjiRepository = kanjiRepository;
    }

    public async Task GetKanji(string userId)
    {
        var kanji = await _kanjiRepository.GetUserKanji(userId);

        if (kanji.Length == 0)
        {
            _waniKaniService.GetWaniKaniKanji(userId);
        }
    }
}