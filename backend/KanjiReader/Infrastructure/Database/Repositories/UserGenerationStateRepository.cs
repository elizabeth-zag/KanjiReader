using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.Repositories;

public class UserGenerationStateRepository(KanjiReaderDbContext dbContext) : IUserGenerationStateRepository
{
    public async Task Update(UserGenerationState state, CancellationToken cancellationToken)
    {
        dbContext.UserGenerationStates.Update(state);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserGenerationState?> Get(string userId, GenerationSourceType sourceType, CancellationToken cancellationToken)
    {
        return await dbContext.UserGenerationStates
            .SingleOrDefaultAsync(r => r.UserId == userId && r.SourceType == sourceType, cancellationToken);
    }
}