namespace KanjiReader.Infrastructure.Database.Models.Events;

public class StartGeneratingData
{
    public GenerationSourceType[] SourceTypes { get; set; }
}