using System.Security.Claims;
using Hangfire;
using KanjiReader.Domain.Common;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Jobs;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Presentation.Dtos.Texts;
using Microsoft.Extensions.Options;

namespace KanjiReader.Domain.TextProcessing;

public class TextService(
    IProcessingResultRepository processingResultRepository, 
    UserAccountService userAccountService,
    KanjiService kanjiService,
    IOptionsMonitor<TextProcessingOptions> textOptions,
    IOptionsMonitor<ThresholdOptions> thresholdOptions)
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
        var currentTextCount = await processingResultRepository.GetCountByUser(user.Id, cancellationToken);
        
        if (currentTextCount >= textOptions.CurrentValue.TextCountLimit)
        {
            throw new InvalidOperationException($"You have reached the limit of {textOptions.CurrentValue.TextCountLimit} texts. " +
                                                $"Please delete some texts before collecting new ones.");
        }

        if (user.LastProcessingTime.HasValue 
            && (DateTime.UtcNow - user.LastProcessingTime.Value).Hours < textOptions.CurrentValue.CooldownHours)
        {
            throw new InvalidOperationException($"You can only collect texts every {textOptions.CurrentValue.CooldownHours} hours." +
                                                $" Please wait before trying again.");
        }
        
        var userKanji = await kanjiService.GetUserKanjiCharacters(user, cancellationToken);

        if (userKanji.Count < textOptions.CurrentValue.MinKanji)
        {
            throw new InvalidOperationException($"You didn't add enough kanji for texts collection. Please add at least " +
                                                $"{textOptions.CurrentValue.MinKanji} kanji");
        }

        foreach (var sourceType in sourceTypes.Where(st => st != GenerationSourceType.Unspecified))
        {
            BackgroundJob.Enqueue<TextProcessingJob>(svc => svc.Execute(
                user.Id, sourceType, textOptions.CurrentValue.TextProcessingCount, null!, CancellationToken.None));
        }
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
        var currentTextCount = await processingResultRepository.GetCountByUser(userId, cancellationToken);
        return Math.Max(textOptions.CurrentValue.TextCountLimit - currentTextCount, 0);
    }
    
    public async Task<(double threshold, bool isUserSet)> GetThreshold(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var user = await userAccountService.GetByClaimsPrincipal(claimsPrincipal);
        return (await GetThreshold(user, cancellationToken), user.Threshold.HasValue);
    }
    
    public async Task<double> GetThreshold(User user, CancellationToken cancellationToken, int? kanjiCount = null)
    {
        if (user.Threshold.HasValue) return user.Threshold.Value;
        kanjiCount ??= (await kanjiService.GetUserKanjiCharacters(user, cancellationToken)).Count;
        
        return CalculateThreshold(kanjiCount.Value);
    }
    
    private double CalculateThreshold(int knownKanji)
    {
        double maxThreshold = thresholdOptions.CurrentValue.MaxThreshold;
        var maxPossibleKanji = 3033;
        
        double normalizedLevel = (double)knownKanji / maxPossibleKanji;
        double ease = Math.Pow(1 - normalizedLevel, 2);
        
        return maxThreshold * ease;
    }
}