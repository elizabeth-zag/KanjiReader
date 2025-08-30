namespace KanjiReader.Infrastructure.Database.Models;

public class Kanji
{
    public int Id { get; set; }
    public char Character { get; set; }
    public string KunReading { get; set; }
    public string OnReading { get; set; }
    public string Meaning { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}