using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Login;

public class GetWaniKaniStagesResponse
{
    public WaniKaniStage[] Stages { get; set; }
}