using System.Text.Json;
using Hangfire;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Jobs;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.EventStream;

namespace KanjiReader.Domain.TextProcessing.Handlers;

public abstract class CommonTextProcessingHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    ITextBroadcaster textBroadcaster,
    KanjiReaderDbContext dbContext)
{
    public async Task Handle(string userId, bool isLastRetry, int textProcessingLeft, CancellationToken cancellationToken)
    {
        var remainingTextCount = await textService.GetRemainingTextCount(userId, cancellationToken);
        if (remainingTextCount <= 0)
        {
            return;
        }
        
        var generationState = await userGenerationStateRepository.Get(userId, GetSourceType(), cancellationToken);
        
        var user = await userAccountService.GetById(userId);

        IReadOnlyCollection<ProcessingResult> processingResults;
        UserGenerationState? updatedGenerationState = null;
        try
        {
            (processingResults, updatedGenerationState) = await ProcessTexts(user, generationState, remainingTextCount, cancellationToken);
        }
        catch
        {
            if (!isLastRetry) throw;
            if (updatedGenerationState != null)
            {
                await userGenerationStateRepository.Update(updatedGenerationState, cancellationToken);
            }
            BackgroundJob.Enqueue<TextProcessingJob>(svc =>
                svc.Execute(userId, GetSourceType(), textProcessingLeft - 1, null!, CancellationToken.None));
            throw;
        }
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            if (updatedGenerationState != null)
            {
                await userGenerationStateRepository.Update(updatedGenerationState, cancellationToken);
            }
            
            if (processingResults.Count > 0)
            {
                await processingResultRepository.Insert(processingResults, cancellationToken);
                if (!user.HasData)
                {
                    await userAccountService.UpdateHasData(user, true);   
                }
            }
            
            remainingTextCount = await textService.GetRemainingTextCount(userId, cancellationToken);
            var isGenerationCompleted = remainingTextCount <= 0 || textProcessingLeft <= 1;

            if (!isGenerationCompleted)
            {
                BackgroundJob.Enqueue<TextProcessingJob>(svc =>
                    svc.Execute(userId, GetSourceType(), textProcessingLeft - 1, null!, CancellationToken.None));
            }

            await transaction.CommitAsync(cancellationToken);

            if (processingResults.Count > 0)
            {
                var resultsDto = processingResults.Select(CommonConverter.Convert).ToArray();
                var json = JsonSerializer.Serialize(resultsDto, JsonDefaults.Options);
                await textBroadcaster.Publish(json, cancellationToken);
            }

            if (isGenerationCompleted)
            {
                await userAccountService.UpdateProcessingTime(user, DateTime.UtcNow);
            }
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    protected abstract Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState? state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount,
        CancellationToken cancellationToken);
    protected abstract GenerationSourceType GetSourceType();
}