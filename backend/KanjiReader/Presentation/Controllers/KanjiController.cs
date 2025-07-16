using KanjiReader.Domain.Kanji;
using KanjiReader.Presentation.Dtos.Kanji;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/kanji")]
public class KanjiController : ControllerBase
{
    private readonly KanjiService _kanjiService;

    public KanjiController(KanjiService kanjiService)
    {
        _kanjiService = kanjiService;
    }

    [HttpPost(nameof(GetKanjiForManualSelection))]
    public async Task<GetKanjiForManualSelectionResponse> GetKanjiForManualSelection(CancellationToken cancellationToken)
    {
        var kanji = await _kanjiService.GetKanjiForManualSelection(cancellationToken);
        
        return new GetKanjiForManualSelectionResponse
        {
            Kanji = kanji.ToArray()
        };
    }

    [HttpPost(nameof(GetKanjiListsForSelection))] 
    public GetKanjiListsForSelectionResponse GetKanjiListsForSelection()
    {
        return new GetKanjiListsForSelectionResponse 
            {
                KanjiLists = _kanjiService.GetKanjiLists().ToArray()
            };
    }

    [HttpPost(nameof(SelectKanji))]
    public async Task<SelectKanjiResponse> SelectKanji(SelectKanjiRequest dto, CancellationToken cancellationToken)
    {
        var kanji = await _kanjiService.SelectKanji(User, dto.Kanji.ToHashSet(), dto.KanjiLists, cancellationToken);

        return new SelectKanjiResponse
        {
            Kanji = kanji.ToArray()
        };
    }
}