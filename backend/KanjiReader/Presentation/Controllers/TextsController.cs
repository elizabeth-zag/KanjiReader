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
            ProcessedTexts = processedTexts.Select(CommonConverter.Convert).ToArray()
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
}