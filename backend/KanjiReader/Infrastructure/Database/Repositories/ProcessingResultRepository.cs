using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class ProcessingResultRepository(KanjiReaderDbContext dbContext) : IProcessingResultRepository
{
    public async Task Insert(IReadOnlyCollection<ProcessingResult> results, CancellationToken cancellationToken)
    {
        await dbContext.ProcessingResults.AddRangeAsync(results, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProcessingResult>> GetByUser(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.ProcessingResults
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<int> GetCountByUser(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.ProcessingResults.CountAsync(r => r.UserId == userId, cancellationToken);
    }

    public async Task Delete(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken)
    {
        await dbContext.ProcessingResults
            .Where(u => textIds.Contains(u.Id))
            .ExecuteDeleteAsync(cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteForUser(string userId, CancellationToken cancellationToken)
    {
        await dbContext.ProcessingResults
            .Where(u => u.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteForUserBySourceTypes(
        string userId, 
        IReadOnlyCollection<GenerationSourceType> sourceTypes, 
        CancellationToken cancellationToken)
    {
        await dbContext.ProcessingResults
            .Where(u => u.UserId == userId && sourceTypes.Contains(u.SourceType))
            .ExecuteDeleteAsync(cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}