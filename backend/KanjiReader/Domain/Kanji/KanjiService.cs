using System.Security.Claims;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects.KanjiLists;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.KanjiApi;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.Dtos.Kanji;

namespace KanjiReader.Domain.Kanji;

public class KanjiService
{
    private readonly WaniKaniService _waniKaniService;
    private readonly IKanjiCacheRepository _kanjiCacheRepository;
    private readonly IKanjiRepository _kanjiRepository;
    private readonly KanjiApiClient _kanjiApiClient;
    private readonly UserAccountService _userAccountService;
    private readonly KanjiReaderDbContext _dbContext;

    public KanjiService(
        WaniKaniService waniKaniService, 
        IKanjiCacheRepository kanjiCacheRepository, 
        KanjiApiClient kanjiApiClient,
        UserAccountService userAccountService, 
        IKanjiRepository kanjiRepository, 
        KanjiReaderDbContext dbContext)
    {
        _waniKaniService = waniKaniService;
        _kanjiCacheRepository = kanjiCacheRepository;
        _kanjiApiClient = kanjiApiClient;
        _userAccountService = userAccountService;
        _kanjiRepository = kanjiRepository;
        _dbContext = dbContext;
    }

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
            kanjiFromLists = await _kanjiApiClient.GetKanjiList(kanjiListTypes, cancellationToken);
        }
        
        var userKanji = new HashSet<char>(selectedKanji.Union(kanjiFromLists));

        var user = await _userAccountService.GetByClaims(claimsPrincipal);
        
        // todo: handle 0 kanji case
        
        var kanji = await _kanjiRepository.GetKanjiByCharacters(userKanji.ToArray(), cancellationToken);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await _kanjiRepository.ClearUserKanji(user.Id, cancellationToken);
            await _kanjiRepository.InsertUserKanji(user.Id, kanji, cancellationToken);

            if (!user.HasData)
            {
                await _userAccountService.UpdateHasData(user, true);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        
        await _kanjiCacheRepository.SetUserKanji(user.Id, selectedKanji);
        
        return selectedKanji;
    }
    
    public Task<IReadOnlySet<char>> GetKanjiForManualSelection(CancellationToken cancellationToken)
    {
        return _kanjiApiClient.GetKanjiList([KanjiListType.Heisig], cancellationToken);
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
        var kanji = await _kanjiCacheRepository.GetUserKanji(user.Id);

        if (!kanji.Any())
        {
            kanji = await _waniKaniService.GetWaniKaniKanji(user.WaniKaniToken, cancellationToken);
            await _kanjiCacheRepository.SetUserKanji(user.Id, kanji);
        }

        return kanji;
    }
}