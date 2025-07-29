using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.TextProcessing.Handlers.WatanocParsing;

public class WatanocParsingHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    WatanocClient watanocClient,
    TextParsingService textParsingService,
    IGenerationRulesService<WatanocParsingData, WatanocParsingBaseData> generationRulesService) 
    : CommonTextProcessingHandler(
        processingResultRepository,
        userAccountService,
        userGenerationStateRepository,
        textService,
        dbContext)
{
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

        var result = await textParsingService.ParseAndValidateText(
            user,
            GetSourceType(),
            remainingTextCount,
            articleUrls,
            watanocClient.ParseHtml,
            cancellationToken);
        
        return (result, generationState);
    }
    
    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Watanoc;
    }
}