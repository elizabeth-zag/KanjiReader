using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class ProcessingResultRepository : IProcessingResultRepository
{
    private readonly KanjiReaderDbContext _dbContext;

    public ProcessingResultRepository(KanjiReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Insert(IReadOnlyCollection<ProcessingResult> results, CancellationToken cancellationToken)
    {
        await _dbContext.ProcessingResults.AddRangeAsync(results, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProcessingResult>> GetByUser(
        string userId, 
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ProcessingResults
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<int> GetCountByUser(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.ProcessingResults.CountAsync(r => r.UserId == userId, cancellationToken);
    }

    public async Task Delete(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken)
    {
        await _dbContext.ProcessingResults
            .Where(u => textIds.Contains(u.Id))
            .ExecuteDeleteAsync(cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteForUser(string userId, CancellationToken cancellationToken)
    {
        await _dbContext.ProcessingResults
            .Where(u => u.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteForUserBySourceTypes(
        string userId, 
        IReadOnlyCollection<GenerationSourceType> sourceTypes, 
        CancellationToken cancellationToken)
    {
        await _dbContext.ProcessingResults
            .Where(u => u.UserId == userId && sourceTypes.Contains(u.SourceType))
            .ExecuteDeleteAsync(cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}