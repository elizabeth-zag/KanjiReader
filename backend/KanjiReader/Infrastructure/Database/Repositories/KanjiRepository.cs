using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class KanjiRepository(KanjiReaderDbContext dbContext) : IKanjiRepository
{
    public async Task<IReadOnlyCollection<Kanji>> GetKanjiByCharacters(IReadOnlyCollection<char> kanji, 
        CancellationToken cancellationToken)
    {
        return await dbContext.Kanji
            .Where(k => kanji.Contains(k.Character))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Kanji>> GetKanjiByUser(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserKanji.Where(uk => uk.UserId == userId).Select(uk => uk.Kanji).ToArrayAsync(cancellationToken);
    }

    public async Task ClearUserKanji(string userId, CancellationToken cancellationToken)
    {
         await dbContext.UserKanji
             .Where(u => u.UserId == userId)
             .ExecuteDeleteAsync(cancellationToken);
         
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task InsertUserKanji(string userId, IReadOnlyCollection<Kanji> kanji, CancellationToken cancellationToken)
    {
        var userKanji = kanji
            .Select(k => new UserKanji
            {
                UserId = userId, 
                KanjiId = k.Id
            }).ToArray();

        await dbContext.UserKanji.AddRangeAsync(userKanji, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}