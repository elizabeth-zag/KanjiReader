using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers;

public abstract class CommonEventHandler(
    IEventRepository eventRepository,
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext)
{
    
    private int _shortMillisecondsDelay = 60000; // todo: config
    private int _longMillisecondsDelay = 900000;
    
    protected async Task Handle(CancellationToken cancellationToken)
    {
        var events = await eventRepository.GetByType(GetEventType(), cancellationToken);
        if (events.Any())
        {
            return; // todo: 0 events
        }
        
        var failedEvents = new List<Event>();

        foreach (var ev in events)
        {
            try
            {
                await Execute(ev.UserId, ev.Data, cancellationToken);
            }
            catch (Exception e)
            {
                if (ev.RetryCount < 3)
                {
                    failedEvents.Add(CreateEventsService.CreateRetryEvent(ev));
                }
                else 
                {
                    // todo: some logging
                }
            }
        }

        await eventRepository.Delete(events, cancellationToken);

        if (failedEvents.Any())
        {
            await eventRepository.Create(failedEvents, cancellationToken);
        }
    }
    
    protected async Task StartProcessingTexts(string userId, string _, CancellationToken cancellationToken)
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

        if (processingResults.Count == 0)
        {
            return;
        }
        
        var isGenerationCompleted = await textService.GetRemainingTextCount(userId, cancellationToken) <= 0;
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await processingResultRepository.Insert(processingResults, cancellationToken);
            await userGenerationStateRepository.Insert(updatedGenerationState, cancellationToken);

            if (!user.HasData)
            {
                await userAccountService.UpdateHasData(user, true);   
            }

            if (!isGenerationCompleted)
            {
                var newEvent = CreateEventsService.CreateNewEvent(userId, GetEventType());
                await eventRepository.Create([newEvent], cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    protected abstract Task Execute(string userId, string stringData, CancellationToken cancellationToken);
    protected abstract Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount,
        CancellationToken cancellationToken);
    protected abstract EventType GetEventType();
    protected abstract GenerationSourceType GetSourceType();
}