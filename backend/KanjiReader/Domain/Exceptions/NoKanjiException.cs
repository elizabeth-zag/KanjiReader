namespace KanjiReader.Domain.Exceptions;

public class NoKanjiException : Exception
{
    public NoKanjiException(string message) : base(message)
    {
    }
}