using System.Text.Json;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.TextProcessing.Handlers.NhkParsing;

public class NhkParsingHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    NhkClient nhkClient,
    TextParsingService textParsingService,
    IGenerationRulesService<NhkParsingData, NhkParsingBaseData> generationRulesService) 
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

        var result = await textParsingService.ParseAndValidateText(
            user,
            GetSourceType(),
            remainingTextCount,
            articleUrls,
            nhkClient.ParseHtml,
            cancellationToken); 
        
        return (result, generationState);
    }
    
    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Nhk;
    }
}