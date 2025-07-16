using System.Security.Claims;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.UserAccount;
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

    public async Task RemoveTexts(IReadOnlyCollection<int> textIds, CancellationToken cancellationToken)
    {
        await _processingResultRepository.SetRemoved(textIds, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<ProcessingResult>> GetProcessedTexts(
        ClaimsPrincipal claimsPrincipal, 
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var user = await _userAccountService.GetByClaims(claimsPrincipal);
        var processingResults = await _processingResultRepository
            .GetByUser(user.Id, false, pageNumber, pageSize, cancellationToken);

        return processingResults;
    }
    
    public async Task<IReadOnlyCollection<ProcessingResult>> GetRemovedTexts(
        ClaimsPrincipal claimsPrincipal, 
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var user = await _userAccountService.GetByClaims(claimsPrincipal);
        var processingResults = await _processingResultRepository
            .GetByUser(user.Id, true, pageNumber, pageSize, cancellationToken);
        
        return processingResults;
    }
}