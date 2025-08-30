using System.Security.Claims;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Exceptions;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.KanjiApi;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Infrastructure.Repositories.Cache;
using KanjiReader.Presentation.Dtos.Kanji;

namespace KanjiReader.Domain.Kanji;

public class KanjiService(
    WaniKaniService waniKaniService,
    IKanjiCacheRepository kanjiCacheRepository,
    KanjiApiClient kanjiApiClient,
    UserAccountService userAccountService,
    IKanjiRepository kanjiRepository,
    KanjiReaderDbContext dbContext)
{
    public async Task<IReadOnlySet<char>> SetUserKanji(
        ClaimsPrincipal claimsPrincipal, 
        IReadOnlySet<char> individualSelectionKanji, 
        IReadOnlyCollection<KanjiListType> kanjiLists, 
        CancellationToken cancellationToken)
    {
        var kanjiListTypes = kanjiLists
            .Where(t => t != KanjiListType.Unspecified)
            .ToArray();
        
        IReadOnlySet<char> kanjiFromLists = kanjiListTypes.Length > 0 
            ? await kanjiApiClient.GetKanjiList(kanjiListTypes, cancellationToken)
            : new HashSet<char>();
        
        var userKanji = new HashSet<char>(individualSelectionKanji.Union(kanjiFromLists));

        var user = await userAccountService.GetByClaimsPrincipal(claimsPrincipal);
        
        var kanji = await kanjiRepository.GetKanjiByCharacters(userKanji.ToArray(), cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await kanjiRepository.ClearUserKanji(user.Id, cancellationToken);
            await kanjiRepository.InsertUserKanji(user.Id, kanji, cancellationToken);

            await userAccountService.UpdateKanjiSourceType(user, KanjiSourceType.ManualSelection);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        
        await kanjiCacheRepository.SetUserKanji(user.Id, userKanji);
        
        return userKanji;
    }
    
    public Task<IReadOnlySet<char>> GetAllKanji(CancellationToken cancellationToken)
    {
        return kanjiApiClient.GetKanjiList([KanjiListType.Heisig], cancellationToken);
    }

    public IReadOnlyCollection<KanjiListResponse> GetKanjiLists()
    {
        return ConstantValues.KanjiListDescriptions
            .Select(d => new KanjiListResponse
            {
                KanjiList = d.Key.ToString(),
                Description = d.Value
            })
            .ToArray();
    }

    public async Task<bool> TryUpdateUserKanjiSource(
        ClaimsPrincipal claimsPrincipal,
        KanjiSourceType kanjiSource,
        CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaimsPrincipal(claimsPrincipal);
        if (IsWaniKaniTokenMissing(kanjiSource, user.WaniKaniToken))
        {
            return false;
        } 
        var kanjiFromUpdatedSource = await GetUserKanjiBySource(user, kanjiSource, cancellationToken);

        var sourceHasKanji = kanjiFromUpdatedSource.Any();
        if (sourceHasKanji)
        {
            await userAccountService.UpdateKanjiSourceType(user, kanjiSource);
            await kanjiCacheRepository.SetUserKanji(user.Id, kanjiFromUpdatedSource.Select(k => k.Character).ToArray());
        }

        return sourceHasKanji;
    }

    public async Task FillKanjiDatabase(CancellationToken cancellationToken)
    {
        var allKanji = await GetAllKanji(cancellationToken);
        var allKanjiData = await kanjiApiClient.GetKanjiData(allKanji, cancellationToken);
        
        var kanji = allKanjiData.Select(CommonConverter.Convert).ToArray();
        await kanjiRepository.InsertKanji(kanji, cancellationToken);
        await kanjiCacheRepository.SetInitialKanji(allKanjiData);
    }

    public async Task<IReadOnlyCollection<char>> GetUserKanjiCharacters(User user, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<char> kanji = await kanjiCacheRepository.GetUserKanjiCharacters(user.Id);

        if (kanji.Any()) return kanji;

        var result = (await GetUserKanjiConsistent(user, cancellationToken)).Select(k => k.Character).ToArray();
        
        if (kanji.Any())
        {
            await kanjiCacheRepository.SetUserKanji(user.Id, kanji.ToArray());
        }
        
        return result;
    }

    public async Task<IReadOnlyCollection<KanjiWithData>> GetUserKanjiFromCache(User user, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<KanjiWithData> kanji = await kanjiCacheRepository.GetUserKanji(user.Id, cancellationToken);

        if (kanji.Any()) return kanji;

        var result = await GetUserKanjiConsistent(user, cancellationToken);
        
        if (kanji.Any())
        {
            await kanjiCacheRepository.SetUserKanji(user.Id, kanji.Select(k => k.Character).ToArray());
        }
        
        return result;
    }
    
    private async Task<IReadOnlyCollection<KanjiWithData>> GetUserKanjiConsistent(User user, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<KanjiWithData> kanji;
        
        if (IsWaniKaniTokenMissing(user.KanjiSourceType, user.WaniKaniToken))
        {
            await userAccountService.UpdateKanjiSourceType(user, KanjiSourceType.ManualSelection);
        }
        
        kanji = await GetUserKanjiBySource(user, user.KanjiSourceType, cancellationToken);
        
        if (user.KanjiSourceType == KanjiSourceType.WaniKani && !kanji.Any())
        {
            throw new NoKanjiException("No kanji found in WaniKani. Please check your WaniKani token and ensure you have mastered some kanji");
        }
        
        return kanji;
    }
    
    private async Task<IReadOnlyCollection<KanjiWithData>> GetUserKanjiBySource(
        User user, 
        KanjiSourceType kanjiSource, 
        CancellationToken cancellationToken)
    {
        return kanjiSource switch
        {
            KanjiSourceType.WaniKani => await GetWaniKaniKanjiWithData(user.WaniKaniToken!, user.WaniKaniStages ?? [], cancellationToken),
            KanjiSourceType.ManualSelection => (await kanjiRepository.GetKanjiByUser(user.Id, cancellationToken))
                .Select(CommonConverter.Convert)
                .ToArray(),
            _ => throw new ArgumentOutOfRangeException(nameof(kanjiSource), kanjiSource, null)
        };
    }

    private async Task<IReadOnlyCollection<KanjiWithData>> GetWaniKaniKanjiWithData(
        string token,
        IReadOnlyCollection<WaniKaniStage> stages,
        CancellationToken cancellationToken)
    {
        var kanjiCharacters = await waniKaniService.GetWaniKaniKanji(token!, stages, cancellationToken);
        return await EnrichKanjiWithData(kanjiCharacters.ToArray(), cancellationToken);
    }
    
    private bool IsWaniKaniTokenMissing(KanjiSourceType kanjiSource, string? token)
    {
        return kanjiSource == KanjiSourceType.WaniKani && string.IsNullOrWhiteSpace(token);
    }
    
    private async Task<IReadOnlyCollection<KanjiWithData>> EnrichKanjiWithData(IReadOnlyCollection<char> kanjiCharacters, CancellationToken cancellationToken)
    {
        if (!kanjiCharacters.Any()) return [];
        
        var kanji = await kanjiRepository.GetKanjiByCharacters(kanjiCharacters, cancellationToken);
        return kanji.Select(CommonConverter.Convert).ToArray();
    }
}