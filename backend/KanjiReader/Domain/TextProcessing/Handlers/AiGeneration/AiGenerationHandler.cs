using System.Text.Json;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.ExternalServices.JapaneseTextSources.GenerativeAI;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.EventStream;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KanjiReader.Domain.TextProcessing.Handlers.AiGeneration;

public class AiGenerationHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    IGenerationRulesService<AiGenerationData, AiGenerationBaseData> generationRulesService,
    TextService textService,
    KanjiReaderDbContext dbContext,
    TextParsingService textParsingService,
    KanjiService kanjiService,
    ITextBroadcaster textBroadcaster,
    IOptionsMonitor<TextProcessingOptions> textOptions,
    GenerativeAiClient client,
    ILogger<AiGenerationHandler> logger) 
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
            ? JsonSerializer.Deserialize<AiGenerationData>(generationState.Data)
            : null;

        var baseData = new AiGenerationBaseData(
            textOptions.CurrentValue.AiGenerationCooldownMinutes,
            textOptions.CurrentValue.AiGenerationTokenLimit);
        
        var generationData = generationRulesService.GetNextState(previousData, baseData);

        if (!generationData.IsAllowed)
        {
            logger.LogError("User {userId} exceeded token limit", user.Id);
            return ([], generationState);
        }
        
        var kanjiCharacters = (await kanjiService.GetUserKanjiCharacters(user, cancellationToken)).ToHashSet();
        var generationResult = await client.GenerateText(kanjiCharacters, cancellationToken);

        textParsingService.ValidateText(kanjiCharacters, generationResult.Title, generationResult.Content, out var ratio, out var unknownKanji);

        var result = new ProcessingResult(
            user.Id, 
            GenerationSourceType.AiGeneration, 
            generationResult.Title,
            generationResult.Content,
            "",
            ratio,
            unknownKanji.ToArray(),
            DateTime.UtcNow);
        
        generationData.AddTokens(generationResult.TokensSpent);
        
        generationState = UserGenerationState.UpdateOrCreateNew(
            generationState, 
            user.Id, 
            GetSourceType(), 
            JsonSerializer.Serialize(generationData));
        
        return ([result], generationState);
    }
    
    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.AiGeneration;
    }
}