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
    [HttpPost(nameof(GetGenerationSources))]
    public GetGenerationSourcesResponse GetGenerationSources()
    {
        return new GetGenerationSourcesResponse { Sources = ["Watanoc"] }; // todo: store sources somewhere
    }

    [HttpPost(nameof(StartGenerating))]
    public async Task StartGenerating(StartGeneratingRequest dto, CancellationToken cancellationToken)
    {
        await textService.StartProcessingTexts(User, dto.SourceTypes.ToHashSet(), cancellationToken);
    }

    [HttpPost(nameof(GetProcessedTexts))]
    public async Task<GetProcessedTextsResponse> GetProcessedTexts(GetProcessedTextsRequest dto, 
        CancellationToken cancellationToken)
    {
        var processedTexts = await textService.GetProcessedTexts(
            User, dto.PageNumber, dto.PageSize, cancellationToken);

        return new GetProcessedTextsResponse
        {
            ProcessedTexts = mapper.Map<ProcessingResultDto[]>(processedTexts)
        };
    }

    [HttpPost(nameof(GetRemovedTexts))]
    public async Task<GetRemovedTextsResponse> GetRemovedTexts(GetRemovedTextsRequest dto,
        CancellationToken cancellationToken)
    {
        var removedTexts = await textService.GetRemovedTexts(
            User, dto.PageNumber, dto.PageSize, cancellationToken);

        return new GetRemovedTextsResponse
        {
            RemovedTexts = mapper.Map<ProcessingResultDto[]>(removedTexts)
        };
    }

    [HttpPost(nameof(RemoveTexts))]
    public async Task RemoveTexts(RemoveTextsRequest dto, CancellationToken cancellationToken)
    {
        await textService.RemoveTexts(dto.TextIds, cancellationToken);
    }
}