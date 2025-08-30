using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.ExternalServices.JapaneseTextSources.GoogleGenerativeAI;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.EventStream;

namespace KanjiReader.Domain.TextProcessing.Handlers.GoogleAiGeneration;

public class GoogleAiGenerationHandler(
    IProcessingResultRepository processingResultRepository,
    UserAccountService userAccountService,
    IUserGenerationStateRepository userGenerationStateRepository,
    TextService textService,
    KanjiReaderDbContext dbContext,
    TextParsingService textParsingService,
    KanjiService kanjiService,
    EmailSender emailSender,
    ITextBroadcaster textBroadcaster,
    GoogleGenerativeAiClient client) 
    : CommonTextProcessingHandler(
        processingResultRepository,
        userAccountService,
        userGenerationStateRepository,
        textService,
        emailSender,
        textBroadcaster,
        dbContext)
{
    protected override async Task<(IReadOnlyCollection<ProcessingResult> results, UserGenerationState? state)> ProcessTexts(
        User user,
        UserGenerationState? generationState,
        int remainingTextCount, 
        CancellationToken cancellationToken)
    {
        var kanjiCharacters = (await kanjiService.GetUserKanjiCharacters(user, cancellationToken)).ToHashSet();
        var (title, text) = await client.GenerateText(kanjiCharacters, cancellationToken);

        textParsingService.ValidateText(kanjiCharacters, title, text, out var ratio, out var unknownKanji);

        var result = new ProcessingResult(
            user.Id, 
            GenerationSourceType.GoogleAiGeneration, 
            title,
            text,
            "",
            ratio,
            unknownKanji.ToArray(),
            DateTime.UtcNow);
        
        return ([result], generationState);
    }
    
    protected override GenerationSourceType GetSourceType()
    {
        return GenerationSourceType.GoogleAiGeneration;
    }
}