using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class ProcessingResultRepository : IProcessingResultRepository
{
    private readonly KanjiReaderDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProcessingResultRepository(KanjiReaderDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task Insert(IReadOnlyCollection<ProcessingResult> results, CancellationToken cancellationToken)
    {
        var resultsDb = _mapper.Map<IEnumerable<ProcessingResultDb>>(results);
        await _dbContext.ProcessingResults.AddRangeAsync(resultsDb, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProcessingResult>> GetByUser(
        string userId, 
        bool isRemoved,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await _dbContext.ProcessingResults
            .Where(r => r.UserId == userId && r.IsRemoved == isRemoved)
            .OrderByDescending(r => r.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyCollection<ProcessingResult>>(result);
    }

    public async Task SetRemoved(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken)
    {
        await _dbContext.ProcessingResults
            .Where(u => textIds.Contains(u.Id))
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(r => r.IsRemoved, true),
                cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}