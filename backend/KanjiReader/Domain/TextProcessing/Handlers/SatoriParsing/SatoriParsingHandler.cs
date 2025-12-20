using System.Text.Json;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.EventStream;
using Microsoft.Extensions.Options;

namespace KanjiReader.Domain.TextProcessing.Handlers.SatoriParsing;

public class SatoriParsingHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    SatoriReaderClient satoriReaderClient,
    TextParsingService textParsingService,
    ITextBroadcaster textBroadcaster,
    IOptionsMonitor<SatoriReaderParsingOptions> options,
    IGenerationRulesService<SatoriParsingData, SatoriParsingBaseData> generationRulesService) 
    : CommonTextProcessingHandler(
        processingResultRepository,
        userAccountService,
        userGenerationStateRepository,
        textService,
        textBroadcaster,
        dbContext)
{
    protected override async Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState? state)> ProcessTexts(
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
        
        var remainingArticleCount = remainingTextCount / options.CurrentValue.ArticlesPerUrl;
        var batchSize = Math.Min(remainingArticleCount, options.CurrentValue.BatchSize);
        
        seriesUrls = seriesUrls.Skip(parsingData.SeriesNumber).Take(batchSize).ToArray();
        
        var articleUrls = await satoriReaderClient.GetArticleUrls(seriesUrls, cancellationToken);

        var result = await textParsingService.ParseAndValidateText(
            user,
            GetSourceType(),
            remainingTextCount,
            articleUrls,
            satoriReaderClient.ParseHtml,
            cancellationToken);
        
        return (result, generationState);
    }

    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.SatoriReader;
    }
}