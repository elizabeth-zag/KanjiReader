using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class TextRepository(KanjiReaderDbContext dbContext) : ITextRepository
{
    public async Task Insert(Text text, CancellationToken cancellationToken)
    {
        dbContext.Texts.Add(text);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Text>> GetAllBySourceType(
        GenerationSourceType type,
        CancellationToken cancellationToken)
    {
        return await dbContext.Texts
            .Where(t => t.SourceType == type)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Text>> GetBySourceType(
        GenerationSourceType type,
        int lastId,
        int take,
        CancellationToken cancellationToken)
    {
        return await dbContext.Texts
            .Where(t => t.SourceType == type && t.Id > lastId)
            .Take(take)
            .ToArrayAsync(cancellationToken);
    }
}