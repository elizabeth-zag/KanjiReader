using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
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

    public async Task<IReadOnlyCollection<char>> GetUserKanji(User user, CancellationToken cancellationToken)
    {
        var kanji = await _kanjiRepository.GetUserKanji(user.Id);

        if (!kanji.Any())
        {
            kanji = await _waniKaniService.GetWaniKaniKanji(user.WaniKaniToken, cancellationToken);
            await _kanjiRepository.SetUserKanji(user.Id, kanji);
        }

        return kanji;
    }
}