using KanjiReader.Domain.Common;
using KanjiReader.Domain.Deletion;
using KanjiReader.Domain.DomainObjects;
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
    DeletionService deletionService,
    TextSavingService textSavingService) : ControllerBase
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

    [HttpGet(nameof(GetProcessedTexts))]
    public async Task<GetProcessedTextsResponse> GetProcessedTexts(CancellationToken cancellationToken)
    {
        var processedTexts = await textService.GetProcessedTexts(User, cancellationToken);

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

    [HttpPost(nameof(FillTextsStorage))]
    public async Task FillTextsStorage(FillTextsStorageRequest dto, CancellationToken cancellationToken)
    {
        if (dto.SourceType != GenerationSourceType.Nhk)
        {
            throw new InvalidOperationException("Only NHK texts are supported at this time");
        }
        await textSavingService.SaveNhkTexts(dto.AuthCookie, cancellationToken);
    }
}