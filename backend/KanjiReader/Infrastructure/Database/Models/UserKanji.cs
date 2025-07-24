namespace KanjiReader.Infrastructure.Database.Models;

public class UserKanji
{
    public string UserId { get; set; }
    public User User { get; set; }
    public int KanjiId { get; set; }
    public Kanji Kanji { get; set; }
}