namespace KanjiReader.Domain.DomainObjects;

public class KanjiWithData
{
    public char Character { get; set; }
    public string KunReadings { get; set; } = "";
    public string OnReadings { get; set; } = "";
    public string Meanings { get; set; } = "";
}