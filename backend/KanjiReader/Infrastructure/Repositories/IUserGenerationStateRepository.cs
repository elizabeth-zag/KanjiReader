using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Infrastructure.Repositories;

public interface IUserGenerationStateRepository
{
    public Task Update(UserGenerationState state, CancellationToken cancellationToken);
    public Task<UserGenerationState?> Get(string userId, GenerationSourceType sourceType, CancellationToken cancellationToken);
}