namespace KanjiReader.Infrastructure.Database.Models;

public class Kanji
{
    public int Id { get; set; }
    public char Character { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
    
    private Kanji() {}
    
    public Kanji(int id, char character)
    {
        Id = id;
        Character = character;
    }
}