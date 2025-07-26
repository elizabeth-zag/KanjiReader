using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.EventData;
using KanjiReader.Domain.DomainObjects.EventData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Text;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.EventHandlers.SatoriParsing;

public class SatoriParsingHandler(IEventRepository eventRepository,
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    SatoriReaderClient satoriReaderClient,
    TextProcessingService textProcessingService,
    IGenerationRulesService<SatoriParsingData, SatoriParsingBaseData> generationRulesService) 
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
            ? JsonSerializer.Deserialize<SatoriParsingData>(generationState.Data)
            : null;

        var seriesUrls = await satoriReaderClient.GetSeriesUrls(cancellationToken);
        var baseData = new SatoriParsingBaseData(seriesUrls.Length);

        var parsingData = generationRulesService.GetNextState(previousData, baseData);
        
        generationState = UserGenerationState.UpdateOrCreateNew(
            generationState, 
            user.Id, 
            GetSourceType(), 
            JsonSerializer.Serialize(parsingData));
        
        // todo: config
        var satoriReaderBatchSize = 4;
        var satoriReaderArticlesPerUrl = 4;
        
        var remainingArticleCount = remainingTextCount / satoriReaderArticlesPerUrl;
        var batchSize = Math.Min(remainingArticleCount, satoriReaderBatchSize);
        
        seriesUrls = seriesUrls.Skip(parsingData.SeriesNumber).Take(batchSize).ToArray();
        
        var articleUrls = await satoriReaderClient.GetArticleUrls(seriesUrls, cancellationToken);

        var result = await textProcessingService.ProcessText(
            user,
            GetSourceType(),
            remainingTextCount,
            articleUrls,
            satoriReaderClient.ParseHtml,
            cancellationToken);
        
        return (result, generationState);
    }

    protected override EventType GetEventType()
    {
        return EventType.SatoriReaderParsing;
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.SatoriReader;
    }
}