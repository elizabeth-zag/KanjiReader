namespace KanjiReader.Infrastructure.Database.Models;

public class Kanji
{
    public int Id { get; set; }
    public string Character { get; set; }
    public string Meaning { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}