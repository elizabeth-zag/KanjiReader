using Hangfire;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Jobs;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.TextProcessing.Handlers;

public abstract class CommonTextProcessingHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext)
{
    public async Task Handle(string userId, CancellationToken cancellationToken)
    {
        var remainingTextCount = await textService.GetRemainingTextCount(userId, cancellationToken);
        if (remainingTextCount <= 0)
        {
            return;
        }
        
        var generationState = await userGenerationStateRepository.Get(userId, GetSourceType(), cancellationToken);
        
        var user = await userAccountService.GetById(userId);
        var (processingResults, updatedGenerationState) = await ProcessTexts(
            user, generationState, remainingTextCount, cancellationToken);
        
        var isGenerationCompleted = await textService.GetRemainingTextCount(userId, cancellationToken) <= 0;
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await userGenerationStateRepository.Insert(updatedGenerationState, cancellationToken);
            
            if (processingResults.Count > 0)
            {
                await processingResultRepository.Insert(processingResults, cancellationToken);
                if (!user.HasData)
                {
                    await userAccountService.UpdateHasData(user, true);   
                }
            }

            if (!isGenerationCompleted)
            {
                BackgroundJob.Enqueue<TextProcessingJob>(svc =>
                    svc.Execute(userId, GetSourceType(), CancellationToken.None));
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    protected abstract Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount,
        CancellationToken cancellationToken);
    protected abstract GenerationSourceType GetSourceType();
}