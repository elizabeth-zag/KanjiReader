using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class ProcessingResultRepository : IProcessingResultRepository
{
    private readonly KanjiReaderDbContext _context;
    private readonly IMapper _mapper;

    public ProcessingResultRepository(KanjiReaderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task Insert(IReadOnlyCollection<ProcessingResult> results, CancellationToken cancellationToken)
    {
        var resultsDb = _mapper.Map<IEnumerable<ProcessingResultDb>>(results);
        await _context.ProcessingResults.AddRangeAsync(resultsDb, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProcessingResult>> GetByUser(string userId, CancellationToken cancellationToken)
    {
        var result = await _context.ProcessingResults
            .Where(r => r.UserId == userId)
            .ToArrayAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyCollection<ProcessingResult>>(result);
    }
}