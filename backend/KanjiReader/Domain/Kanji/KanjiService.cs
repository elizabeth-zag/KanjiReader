using System.Security.Claims;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.KanjiLists;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.KanjiApi;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
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
    public async Task<IReadOnlySet<char>> SelectKanji(
        ClaimsPrincipal claimsPrincipal, 
        IReadOnlySet<char> selectedKanji, 
        IReadOnlyCollection<KanjiListType> kanjiLists, 
        CancellationToken cancellationToken)
    {
        var kanjiListTypes = kanjiLists
            .Where(t => t != KanjiListType.Unspecified)
            .ToArray();
        
        IReadOnlySet<char> kanjiFromLists = new HashSet<char>();

        if (kanjiListTypes.Length > 0)
        {
            kanjiFromLists = await kanjiApiClient.GetKanjiList(kanjiListTypes, cancellationToken);
        }
        
        var userKanji = new HashSet<char>(selectedKanji.Union(kanjiFromLists));

        if (userKanji.Count == 0)
        {
            throw new ArgumentException("No kanji were provided by the user");
        }

        var user = await userAccountService.GetByClaims(claimsPrincipal);
        
        var kanji = await kanjiRepository.GetKanjiByCharacters(userKanji.ToArray(), cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await kanjiRepository.ClearUserKanji(user.Id, cancellationToken);
            await kanjiRepository.InsertUserKanji(user.Id, kanji, cancellationToken);

            if (!user.HasData)
            {
                await userAccountService.UpdateHasData(user, true);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        
        await kanjiCacheRepository.SetUserKanji(user.Id, selectedKanji);
        
        return selectedKanji;
    }
    
    public Task<IReadOnlySet<char>> GetKanjiForManualSelection(CancellationToken cancellationToken)
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
    
    public async Task<IReadOnlyCollection<char>> GetUserKanji(User user, CancellationToken cancellationToken)
    {
        var kanji = await kanjiCacheRepository.GetUserKanji(user.Id);

        if (!kanji.Any())
        {
            var kanjiSource = user.KanjiSourceType;
            if (kanjiSource == KanjiSourceType.WaniKani && !string.IsNullOrWhiteSpace(user.WaniKaniToken))
            {
                kanji = await waniKaniService.GetWaniKaniKanji(user.WaniKaniToken, cancellationToken);
                
            }
            else
            {
                var userKanji = await kanjiRepository.GetKanjiByUser(user.Id, cancellationToken);
                kanji = userKanji.Select(uk => uk.Character).ToHashSet();
            }
            await kanjiCacheRepository.SetUserKanji(user.Id, kanji);
        }

        return kanji;
    }
}