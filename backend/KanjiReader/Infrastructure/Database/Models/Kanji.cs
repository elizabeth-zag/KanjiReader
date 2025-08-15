namespace KanjiReader.Infrastructure.Database.Models;

public class Kanji
{
    public int Id { get; set; }
    public char Character { get; set; }
    public string KunReading { get; set; }
    public string OnReading { get; set; }
    public string Meaning { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; } // todo: don't take
    
    public Kanji() {}
    
    public Kanji(int id, char character, string kunReading, string onReading, string meaning)
    {
        Id = id;
        Character = character;
        KunReading = kunReading;
        OnReading = onReading;
        Meaning = meaning;
    }
}