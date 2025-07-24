using System.Security.Claims;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.Text;

public class TextService
{
    private readonly IProcessingResultRepository _processingResultRepository;
    private readonly UserAccountService _userAccountService;

    public TextService(IProcessingResultRepository processingResultRepository, UserAccountService userAccountService)
    {
        _processingResultRepository = processingResultRepository;
        _userAccountService = userAccountService;
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
        return Math.Min(textCountLimit - currentTextCount, 0);
    }
}