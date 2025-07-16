namespace KanjiReader.Domain.DomainObjects;

public class Kanji
{
    public int Id;
    public string Character;

    public Kanji(string character)
    {
        Character = character;
    }
}