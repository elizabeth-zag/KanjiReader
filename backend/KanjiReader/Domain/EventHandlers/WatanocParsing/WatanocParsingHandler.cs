using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.DomainObjects.EventData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers.WatanocParsing;

public class WatanocParsingHandler(
    IEventRepository eventRepository,
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    WatanocClient watanocClient,
    TextProcessingService textProcessingService,
    IGenerationRulesService<WatanocParsingData, WatanocParsingBaseData> generationRulesService) 
    : CommonEventHandler(eventRepository,
        processingResultRepository,
        userAccountService,
        userGenerationStateRepository,
        textService,
        dbContext)
{
    protected override async Task Execute(string userId, string stringData, CancellationToken cancellationToken)
    {
        await StartProcessingTexts(userId, stringData, cancellationToken);
    }
    
    protected override EventType GetEventType()
    {
        return EventType.WatanocParsing;
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Watanoc;
    }

    protected override async Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount, 
        CancellationToken cancellationToken)
    {
        var previousData = generationState != null
            ? JsonSerializer.Deserialize<WatanocParsingData>(generationState.Data)
            : null;

        var parsingData = generationRulesService.GetNextState(previousData, new WatanocParsingBaseData());
        generationState = UserGenerationState.UpdateOrCreateNew(
            generationState, 
            user.Id, 
            GetSourceType(), 
            JsonSerializer.Serialize(parsingData));
        
        var articleUrls = await watanocClient.GetArticleUrls(parsingData.Category, parsingData.PageNumber, cancellationToken);

        var result = await textProcessingService.ProcessText(
            user,
            GetSourceType(),
            remainingTextCount,
            articleUrls,
            watanocClient.ParseHtml,
            cancellationToken);
        
        return (result, generationState);
    }
}