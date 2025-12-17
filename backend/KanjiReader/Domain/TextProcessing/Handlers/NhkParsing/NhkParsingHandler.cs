using System.Text.Json;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.EventStream;
using Microsoft.Extensions.Options;

namespace KanjiReader.Domain.TextProcessing.Handlers.NhkParsing;

public class NhkParsingHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    ITextRepository textRepository,
    TextParsingService textParsingService,
    EmailSender emailSender,
    ITextBroadcaster textBroadcaster,
    IOptionsMonitor<NhkOptions> options,
    IGenerationRulesService<NhkParsingData, NhkParsingBaseData> generationRulesService) 
    : CommonTextProcessingHandler(
        processingResultRepository,
        userAccountService,
        userGenerationStateRepository,
        textService,
        emailSender,
        textBroadcaster,
        dbContext)
{
    private IReadOnlyCollection<Text> _texts = [];
    private int _lastId;
    protected override async Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState? state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount, 
        CancellationToken cancellationToken) 
    {
        var previousData = generationState != null
            ? JsonSerializer.Deserialize<NhkParsingData>(generationState.Data)
            : null;

        _texts = await textRepository.GetBySourceType(
            GenerationSourceType.Nhk,
            previousData?.LastId ?? 0,
            options.CurrentValue.BatchSize,
            cancellationToken);

        if (_texts.Count == 0 && previousData?.LastId > 0)
        {
            _texts = await textRepository.GetBySourceType(
                GenerationSourceType.Nhk,
                0,
                options.CurrentValue.BatchSize,
                cancellationToken);
        }
        
        var result = await textParsingService.ParseAndValidateText(
            user,
            GetSourceType(),
            remainingTextCount,
            _texts.Select(t => t.Url).ToArray(),
            GetTextByUrl,
            cancellationToken);

        var baseData = new NhkParsingBaseData(_lastId);
        var parsingData = generationRulesService.GetNextState(previousData, baseData);
        
        generationState = UserGenerationState.UpdateOrCreateNew(
            generationState, 
            user.Id, 
            GetSourceType(), 
            JsonSerializer.Serialize(parsingData));
        
        return (result, generationState);
    }
    
    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.Nhk;
    }

    private Task<(string, string)> GetTextByUrl(string url, CancellationToken cancellationToken)
    {
        var text = _texts.FirstOrDefault(x => x.Url == url);
        var result = text is null ? (string.Empty, string.Empty) : (text.Title, text.Content);
        _lastId = text?.Id ?? 0;
        return Task.FromResult(result);
    }
}