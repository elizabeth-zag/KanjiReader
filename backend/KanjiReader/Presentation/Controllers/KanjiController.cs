using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Presentation.Dtos.Kanji;
using KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/kanji")]
public class KanjiController(KanjiService kanjiService, UserAccountService userAccountService) : ControllerBase
{
    [HttpGet(nameof(GetKanjiForManualSelection))]
    public async Task<GetKanjiForManualSelectionResponse> GetKanjiForManualSelection(CancellationToken cancellationToken)
    {
        var kanji = await kanjiService.GetAllKanji(cancellationToken);
        
        return new GetKanjiForManualSelectionResponse
        {
            Kanji = kanji.ToArray()
        };
    }

    [HttpGet(nameof(GetKanjiListsForSelection))] 
    public GetKanjiListsForSelectionResponse GetKanjiListsForSelection()
    {
        return new GetKanjiListsForSelectionResponse 
            {
                KanjiLists = kanjiService.GetKanjiLists().ToArray()
            };
    }

    [HttpPost(nameof(SetSelectedKanji))]
    public async Task<SetSelectedKanjiResponse> SetSelectedKanji(SetSelectedKanjiRequest dto, CancellationToken cancellationToken)
    {
        var kanji = await kanjiService.SetUserKanji(User, dto.Kanji.ToHashSet(), dto.KanjiLists, cancellationToken);

        return new SetSelectedKanjiResponse
        {
            Kanji = kanji.ToArray()
        };
    }

    [HttpGet(nameof(GetUserKanji))]
    public async Task<GetUserKanjiResponse> GetUserKanji(CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaimsPrincipal(User);
        var kanji = await kanjiService.GetUserKanjiFromCache(user, cancellationToken);

        return new GetUserKanjiResponse
        {
            Kanji = kanji.ToArray(),
            KanjiSourceType = user.KanjiSourceType.ToString(),
        };
    }
    
    [HttpPost(nameof(TryUpdateKanjiSource))]
    public async Task<TryUpdateKanjiSourceResponse> TryUpdateKanjiSource(
        TryUpdateKanjiSourceRequest dto, 
        CancellationToken cancellationToken)
    {
        var result = await kanjiService.TryUpdateUserKanjiSource(User, dto.KanjiSourceType, cancellationToken);
        return new TryUpdateKanjiSourceResponse { Success = result };
    }
    
    [HttpPost(nameof(FillKanjiDatabase))]
    public async Task FillKanjiDatabase(CancellationToken cancellationToken)
    {
        await kanjiService.FillKanjiDatabase(User, cancellationToken);
    }
}