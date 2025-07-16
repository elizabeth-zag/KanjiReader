namespace KanjiReader.Infrastructure.Database.Models;

public class KanjiDb
{
    public int Id { get; set; }
    public char Character { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}