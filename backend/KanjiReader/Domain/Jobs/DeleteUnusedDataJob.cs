using KanjiReader.Domain.Deletion;
using KanjiReader.Domain.UserAccount;
using Microsoft.Extensions.Logging;

namespace KanjiReader.Domain.Jobs;

public class DeleteUnusedDataJob(
    UserAccountService userAccountService, 
    DeletionService deletionService, 
    ILogger<DeleteUnusedDataJob> logger)
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var inactiveUsers = await userAccountService.GetInactiveUsers(cancellationToken);
        var inactiveUsersWithData = inactiveUsers.Where(u => u.HasData).ToArray();
        
        if (inactiveUsersWithData.Any())
        {
            foreach (var user in inactiveUsers)
            {
                await deletionService.DeleteUserTexts(user, cancellationToken);
                logger.LogWarning("Data deleted for user {UserId}", user.Id);
                await userAccountService.UpdateHasData(user, false);
            }
        }
    }
}