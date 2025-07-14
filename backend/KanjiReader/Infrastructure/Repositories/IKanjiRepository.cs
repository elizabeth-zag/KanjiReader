namespace KanjiReader.Infrastructure.Repositories;

public interface IKanjiRepository
{
    Task SetUserKanji(string userId, IReadOnlySet<char> kanji);
    Task<IReadOnlySet<char>> GetUserKanji(string userId);
}