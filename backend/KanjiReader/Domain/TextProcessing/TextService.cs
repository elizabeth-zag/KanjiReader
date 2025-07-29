using System.Security.Claims;
using Hangfire;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.Jobs;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.TextProcessing;

public class TextService
{
    private readonly IProcessingResultRepository _processingResultRepository;
    private readonly UserAccountService _userAccountService;

    public TextService(IProcessingResultRepository processingResultRepository, UserAccountService userAccountService)
    {
        _processingResultRepository = processingResultRepository;
        _userAccountService = userAccountService;
    }
    
    public async Task StartProcessingTexts(
        ClaimsPrincipal claimsPrincipal, 
        IReadOnlySet<GenerationSourceType> sourceTypes, 
        CancellationToken cancellationToken)
    {
        var user = await _userAccountService.GetByClaims(claimsPrincipal);
        
        var textCountLimit = 30; // todo: move to config
        var currentTextCount = await _processingResultRepository.GetCountByUser(user.Id, cancellationToken);
        
        if (currentTextCount >= textCountLimit)
        {
            throw new InvalidOperationException($"You have reached the limit of {textCountLimit} texts. Please delete some texts before generating new ones.");
        }

        foreach (var sourceType in sourceTypes.Where(st => st != GenerationSourceType.Unspecified))
        {
            BackgroundJob.Enqueue<TextProcessingJob>(svc => svc.Execute(user.Id, sourceType, CancellationToken.None));
        }
    }
    
    public async Task<IReadOnlyCollection<ProcessingResult>> GetProcessedTexts(
        ClaimsPrincipal claimsPrincipal, 
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var user = await _userAccountService.GetByClaims(claimsPrincipal);
        var processingResults = await _processingResultRepository
            .GetByUser(user.Id, pageNumber, pageSize, cancellationToken);

        return processingResults;
    }
    
    public async Task<int> GetRemainingTextCount(string userId, CancellationToken cancellationToken)
    {
        var textCountLimit = 30; // todo: move to config
        var currentTextCount = await _processingResultRepository.GetCountByUser(userId, cancellationToken);
        return Math.Max(textCountLimit - currentTextCount, 0);
    }
}