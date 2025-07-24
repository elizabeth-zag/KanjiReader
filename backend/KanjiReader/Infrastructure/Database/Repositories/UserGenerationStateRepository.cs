using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class UserGenerationStateRepository : IUserGenerationStateRepository
{
    private readonly KanjiReaderDbContext _dbContext;

    public UserGenerationStateRepository(KanjiReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Insert(UserGenerationState state, CancellationToken cancellationToken)
    {
        _dbContext.UserGenerationStates.Update(state);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserGenerationState?> Get(string userId, GenerationSourceType sourceType, CancellationToken cancellationToken)
    {
        return await _dbContext.UserGenerationStates
            .SingleOrDefaultAsync(r => r.UserId == userId && r.SourceType == sourceType, cancellationToken);
    }
}