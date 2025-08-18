using System.Security.Claims;
using Hangfire;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Jobs;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.Dtos.Texts;

namespace KanjiReader.Domain.TextProcessing;

public class TextService(
    IProcessingResultRepository processingResultRepository, 
    UserAccountService userAccountService,
    KanjiService kanjiService)
{
    public static GenerationSourceDto[] GetGenerationSources()
    {
        return Enum.GetValues<GenerationSourceType>()
            .Where(s => s != GenerationSourceType.Unspecified)
            .Select(st => new GenerationSourceDto
            {
                Value = st.ToString(),
                Name = ConstantValues.SourceTypeNames[st],
                Description = ConstantValues.SourceTypeDescriptions[st]
            })
            .ToArray();
    }
    
    public async Task StartCollectingTexts(
        ClaimsPrincipal claimsPrincipal, 
        IReadOnlySet<GenerationSourceType> sourceTypes, 
        CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaimsPrincipal(claimsPrincipal);
        
        var textCountLimit = 30; // todo: move to config
        var textProcessingLeft = 30; // todo: move to config
        var cooldownHours = 2; // todo: move to config
        var currentTextCount = await processingResultRepository.GetCountByUser(user.Id, cancellationToken);
        
        if (currentTextCount >= textCountLimit)
        {
            throw new InvalidOperationException($"You have reached the limit of {textCountLimit} texts. Please delete some texts before collecting new ones.");
        }

        if (user.LastProcessingTime.HasValue && (DateTime.UtcNow - user.LastProcessingTime.Value).Hours < cooldownHours)
        {
            throw new InvalidOperationException($"You can only collect texts every {cooldownHours} hours. Please wait before trying again.");
        }

        foreach (var sourceType in sourceTypes.Where(st => st != GenerationSourceType.Unspecified))
        {
            BackgroundJob.Enqueue<TextProcessingJob>(svc => svc.Execute(user.Id, sourceType, textProcessingLeft, null!, CancellationToken.None));
        }
        
        await userAccountService.UpdateProcessingTime(user, DateTime.UtcNow);
    }
    
    public async Task<IReadOnlyCollection<ProcessingResult>> GetProcessedTexts(
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaimsPrincipal(claimsPrincipal);
        var processingResults = await processingResultRepository
            .GetByUser(user.Id, cancellationToken);

        return processingResults;
    }
    
    public async Task<int> GetRemainingTextCount(string userId, CancellationToken cancellationToken)
    {
        var textCountLimit = 30; // todo: move to config
        var currentTextCount = await processingResultRepository.GetCountByUser(userId, cancellationToken);
        return Math.Max(textCountLimit - currentTextCount, 0);
    }
    
    public async Task<double> GetThreshold(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaimsPrincipal(claimsPrincipal);
        var kanjiCharacters = (await kanjiService.GetUserKanjiFromCache(user, cancellationToken)).ToHashSet();
        
        return TextParsingService.GetUserThreshold(user, kanjiCharacters.Count);
    }
}