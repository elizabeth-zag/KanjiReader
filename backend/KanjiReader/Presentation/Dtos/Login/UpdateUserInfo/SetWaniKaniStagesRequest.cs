using KanjiReader.Domain.DomainObjects;

namespace KanjiReader.Presentation.Dtos.Login.UpdateUserInfo;

public class SetWaniKaniStagesRequest
{
    public WaniKaniStage[] Stages { get; set; }
}