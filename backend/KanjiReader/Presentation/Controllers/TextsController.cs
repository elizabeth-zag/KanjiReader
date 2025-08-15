using KanjiReader.Domain.Common;
using KanjiReader.Domain.Deletion;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Presentation.Dtos.Texts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/texts")]
public class TextsController(
    TextService textService, 
    DeletionService deletionService) : ControllerBase
{
    [HttpGet(nameof(GetGenerationSources))]
    public GetGenerationSourcesResponse GetGenerationSources()
    {
        return new GetGenerationSourcesResponse { Sources = TextService.GetGenerationSources() };
    }

    [HttpPost(nameof(StartCollecting))]
    public async Task StartCollecting(StartCollectingRequest dto, CancellationToken cancellationToken)
    {
        await textService.StartCollectingTexts(User, dto.Sources.ToHashSet(), cancellationToken);
    }

    [HttpPost(nameof(GetProcessedTexts))]
    public async Task<GetProcessedTextsResponse> GetProcessedTexts(GetProcessedTextsRequest dto, 
        CancellationToken cancellationToken)
    {
        var processedTexts = await textService.GetProcessedTexts(
            User, dto.PageNumber, dto.PageSize, cancellationToken);
        var allTextsCount = await textService.GetCountByUser(User, cancellationToken);

        return new GetProcessedTextsResponse
        {
            ProcessedTexts = processedTexts.Select(CommonConverter.Convert).ToArray(),
            AllTextsCount = allTextsCount
        };
    }

    [HttpPost(nameof(RemoveTexts))]
    public async Task RemoveTexts(RemoveTextsRequest dto, CancellationToken cancellationToken)
    {
        await deletionService.RemoveTexts(dto.TextIds, cancellationToken);
    }

    [HttpPost(nameof(RemoveTextsBySourceType))]
    public async Task RemoveTextsBySourceType(RemoveTextsBySourceTypesRequest dto, CancellationToken cancellationToken)
    {
        await deletionService.RemoveUserTextsBySourceType(User, dto.SourceTypes, cancellationToken);
    }

    [HttpPost(nameof(GetUserThreshold))]
    public async Task<GetUserThresholdResponse> GetUserThreshold(CancellationToken cancellationToken)
    {
        var result = await textService.GetThreshold(User, cancellationToken);
        
        return new GetUserThresholdResponse
        {
            Threshold = result
        };
    }
}