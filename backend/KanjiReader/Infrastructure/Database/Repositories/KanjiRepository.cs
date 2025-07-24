using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class KanjiRepository : IKanjiRepository
{
    private readonly KanjiReaderDbContext _dbContext;

    public KanjiRepository(KanjiReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Kanji>> GetKanjiByCharacters(IReadOnlyCollection<char> kanji, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.Kanji
            .Where(k => kanji.Contains(k.Character))
            .ToListAsync(cancellationToken);
    }

    public async Task ClearUserKanji(string userId, CancellationToken cancellationToken)
    {
         await _dbContext.UserKanji
             .Where(u => u.UserId == userId)
             .ExecuteDeleteAsync(cancellationToken);
         
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task InsertUserKanji(string userId, IReadOnlyCollection<Kanji> kanji, CancellationToken cancellationToken)
    {
        var userKanji = kanji
            .Select(k => new UserKanji
            {
                UserId = userId, 
                KanjiId = k.Id
            }).ToArray();

        await _dbContext.UserKanji.AddRangeAsync(userKanji, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}