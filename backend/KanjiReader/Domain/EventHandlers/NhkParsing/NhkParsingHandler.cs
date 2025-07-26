
using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.DomainObjects.EventData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers.NhkParsing;

public class NhkParsingHandler(IEventRepository eventRepository,
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    NhkClient nhkClient,
    TextProcessingService textProcessingService,
    IGenerationRulesService<NhkParsingData, NhkParsingBaseData> generationRulesService) 
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
    
    protected override async Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount, 
        CancellationToken cancellationToken) 
    {
        var previousData = generationState != null
            ? JsonSerializer.Deserialize<NhkParsingData>(generationState.Data)
            : null;

        var articleUrlsByDate = await nhkClient.GetArticleUrls(cancellationToken);
        var baseData = new NhkParsingBaseData(articleUrlsByDate.Keys.ToArray());

        var parsingData = generationRulesService.GetNextState(previousData, baseData);
        
        generationState = UserGenerationState.UpdateOrCreateNew(
            generationState, 
            user.Id, 
            GetSourceType(), 
            JsonSerializer.Serialize(parsingData));
        
        var articleUrls = articleUrlsByDate[parsingData.CurrentDate]; // todo: handle empty result

        var result = await textProcessingService.ProcessText(
            user,
            GetSourceType(),
            remainingTextCount,
            articleUrls,
            nhkClient.ParseHtml,
            cancellationToken); 
        
        return (result, generationState);
    }

    protected override EventType GetEventType()
    {
        return EventType.NhkParsing;
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Nhk;
    }
}