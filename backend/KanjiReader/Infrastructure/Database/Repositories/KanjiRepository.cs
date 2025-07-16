using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class KanjiRepository : IKanjiRepository
{
    private readonly KanjiReaderDbContext _dbContext;
    private readonly IMapper _mapper;

    public KanjiRepository(KanjiReaderDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Kanji>> GetKanjiByCharacters(IReadOnlyCollection<char> kanji, 
        CancellationToken cancellationToken)
    {
        var result = await _dbContext.Kanji
            .Where(k => kanji.Contains(k.Character))
            .ToListAsync(cancellationToken);

        return _mapper.Map<Kanji[]>(result);
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

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }
}