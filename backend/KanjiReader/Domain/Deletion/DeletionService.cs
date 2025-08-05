using System.Security.Claims;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.Deletion;

public class DeletionService(
    IProcessingResultRepository processingResultRepository,
    IKanjiRepository kanjiRepository,
    UserAccountService userAccountService)
{
    public async Task<bool> DeleteUser(ClaimsPrincipal userPrincipal, string password, CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaims(userPrincipal);
        var isDeletionSuccessful = await userAccountService.DeleteUserAccount(userPrincipal, password);
        
        if (!isDeletionSuccessful)
        {
            return false;
        }

        await DeleteUserData(user, cancellationToken);
        return true;
    }
    
    public async Task DeleteUserData(User user, CancellationToken cancellationToken)
    {
        await kanjiRepository.ClearUserKanji(user.Id, cancellationToken);
        await processingResultRepository.DeleteForUser(user.Id, cancellationToken);
    }
    
    public async Task RemoveTexts(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken)
    {
        await processingResultRepository.Delete(textIds, cancellationToken);
    }
    
    public async Task RemoveUserTextsBySourceType(
        ClaimsPrincipal claimsPrincipal, 
        IReadOnlyCollection<GenerationSourceType> sourceTypes, 
        CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaims(claimsPrincipal);
        await processingResultRepository.DeleteForUserBySourceTypes(user.Id, sourceTypes, cancellationToken);
    }
}