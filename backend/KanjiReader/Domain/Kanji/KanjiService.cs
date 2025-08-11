using System.Security.Claims;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.KanjiLists;
using KanjiReader.Domain.Exceptions;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.KanjiApi;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.Dtos.Kanji;
using KanjiDb = KanjiReader.Infrastructure.Database.Models.Kanji;

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

        if (userKanji.Count == 0)
        {
            throw new ArgumentException("No kanji were provided by the user");
        }

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
        return KanjiListsDescription.KanjiListDescriptions
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
            await kanjiCacheRepository.SetUserKanji(user.Id, kanjiFromUpdatedSource);
        }

        return sourceHasKanji;
    }

    public async Task FillKanjiDatabase(CancellationToken cancellationToken)
    {
        var allKanji = await GetAllKanji(cancellationToken);
        var kanji = allKanji.Select(k => new KanjiDb { Character = k }).ToArray();
        await kanjiRepository.InsertKanji(kanji, cancellationToken);
    }
    
    public async Task<IReadOnlySet<char>> GetUserKanji(User user, CancellationToken cancellationToken)
    {
        IReadOnlySet<char> kanji = await kanjiCacheRepository.GetUserKanji(user.Id);

        if (kanji.Any()) return kanji;
        
        if (IsWaniKaniTokenMissing(user.KanjiSourceType, user.WaniKaniToken))
        {
            await userAccountService.UpdateKanjiSourceType(user, KanjiSourceType.ManualSelection);
        }

        kanji = await GetUserKanjiBySource(user, user.KanjiSourceType, cancellationToken);
        
        if (user.KanjiSourceType == KanjiSourceType.WaniKani && !kanji.Any())
        {
            throw new NoKanjiException("No kanji found in WaniKani. Please check your WaniKani token and ensure you have mastered some kanji");
        }
        
        if (kanji.Any())
        {
            await kanjiCacheRepository.SetUserKanji(user.Id, kanji);
        }

        return kanji;
    }

    private async Task<IReadOnlySet<char>> GetUserKanjiBySource(
        User user, 
        KanjiSourceType kanjiSource, 
        CancellationToken cancellationToken)
    {
        IReadOnlySet<char> kanji;
        if (kanjiSource == KanjiSourceType.WaniKani)
        {
            kanji = await waniKaniService.GetWaniKaniKanji(user.WaniKaniToken!, cancellationToken);
        }
        else
        {
            var userKanji = await kanjiRepository.GetKanjiByUser(user.Id, cancellationToken);
            kanji = userKanji.Select(uk => uk.Character).ToHashSet();
        }

        return kanji;
    }

    private bool IsWaniKaniTokenMissing(KanjiSourceType kanjiSource, string? token)
    {
        return kanjiSource == KanjiSourceType.WaniKani && string.IsNullOrWhiteSpace(token);
    }
}