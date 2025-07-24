using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk;
using KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KanjiReader.Domain.EventHandlers;

public abstract class CommonEventHandler(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private IEventRepository _eventRepository;
    private IProcessingResultRepository _processingResultRepository;
    private UserAccountService _userAccountService;
    private KanjiService _kanjiService;
    private SatoriReaderClient _satoriReaderClient;
    private IGenerationRulesService<SatoriParsingData> _generationRulesService;
    private IUserGenerationStateRepository _userGenerationStateRepository;
    private TextService _textService;
    private KanjiReaderDbContext _dbContext;
    
    private int _shortMillisecondsDelay = 60000; // todo: config
    private int _longMillisecondsDelay = 900000;
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var idlingCount = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            _eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            _processingResultRepository = scope.ServiceProvider.GetRequiredService<IProcessingResultRepository>();
            
            var events = await _eventRepository.GetByType(GetEventType(), cancellationToken);
            if (events.Any())
            {
                await Task.Delay(idlingCount < 10 
                    ? _shortMillisecondsDelay 
                    : _longMillisecondsDelay, cancellationToken); // todo: config
                
                idlingCount++;
                continue;
            }
            idlingCount = 0;
            
            SetScopedDependencies(scope);
            
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

            await _eventRepository.Delete(events, cancellationToken);

            if (failedEvents.Any())
            {
                await _eventRepository.Create(failedEvents, cancellationToken);
            }
        }
    }
    
    protected async Task StartProcessingTexts(string userId, string _, CancellationToken cancellationToken)
    {
        var remainingTextCount = await _textService.GetRemainingTextCount(userId, cancellationToken);
        if (remainingTextCount <= 0)
        {
            return;
        }
        
        var generationState = await _userGenerationStateRepository.Get(userId, GetSourceType(), cancellationToken);
        
        var user = await _userAccountService.GetById(userId);
        var (processingResults, updatedGenerationState) = await ProcessTexts(
            user, generationState, remainingTextCount, cancellationToken);

        if (processingResults.Length == 0)
        {
            return;
        }
        
        var isGenerationCompleted = await _textService.GetRemainingTextCount(userId, cancellationToken) <= 0;
        
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await _processingResultRepository.Insert(processingResults, cancellationToken);
            await _userGenerationStateRepository.Insert(updatedGenerationState, cancellationToken);

            if (!user.HasData)
            {
                await _userAccountService.UpdateHasData(user, true);   
            }

            if (!isGenerationCompleted)
            {
                var newEvent = CreateEventsService.CreateNewEvent(userId, GetEventType());
                await _eventRepository.Create([newEvent], cancellationToken);
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
    protected abstract Task<(ProcessingResult[] results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount,
        CancellationToken cancellationToken);
    protected abstract void SetScopedDependencies(IServiceScope scope);
    protected abstract EventType GetEventType();
    protected abstract GenerationSourceType GetSourceType();
}