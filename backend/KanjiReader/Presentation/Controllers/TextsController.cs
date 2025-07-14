using KanjiReader.Domain.EventHandlers;
using KanjiReader.Presentation.Dtos.Login;
using KanjiReader.Presentation.Dtos.Texts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanjiReader.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/texts")]
public class TextsController(CreateEventsService createEventsService) : ControllerBase
{

    [HttpPost(nameof(GetGenerationSources))]
    public Task<GetGenerationSourcesResponse> GetGenerationSources()
    {
        return Task.FromResult(new GetGenerationSourcesResponse { Sources = ["Watanoc"] }); // todo: store sources somewhere
    }

    [HttpPost(nameof(StartGenerating))]
    public async Task StartGenerating(StartGeneratingRequest dto, CancellationToken cancellationToken)
    {
        await createEventsService.CreateStartGeneratingEvents(User, dto.SourceTypes.ToHashSet(), cancellationToken);
    }

    [HttpPost(nameof(StartGenerating))]
    public async Task GetProcessedText(StartGeneratingRequest dto, CancellationToken cancellationToken)
    {
        await createEventsService.CreateStartGeneratingEvents(User, dto.SourceTypes.ToHashSet(), cancellationToken);
    }
}