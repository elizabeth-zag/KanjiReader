using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Infrastructure.Database.Models;

public class UserGenerationState
{
    public int Id { get; init; }
    public string UserId { get; init; }
    public User User { get; init; }
    public GenerationSourceType SourceType { get; init; }
    public string Data { get; private set; }
    
    public UserGenerationState UpdateData(string newData)
    {
        Data = newData;
        return this;
    }
    
    public static UserGenerationState UpdateOrCreateNew(
        UserGenerationState? generationState, 
        string userId, 
        GenerationSourceType sourceType, 
        string data)
    {
        return generationState?.UpdateData(data)
               ?? new UserGenerationState(
                   userId,
                   sourceType,
                   data);
    }

    public UserGenerationState() {}
    
    public UserGenerationState(string userId, GenerationSourceType sourceType, string data)
    {
        UserId = userId;
        SourceType = sourceType;
        Data = data;
    }
}