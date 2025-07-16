using KanjiReader.Domain.DomainObjects;
using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Infrastructure.Database.Models;

public class User : IdentityUser
{
    public KanjiSourceType KanjiSourceType { get; set; }
    public string? WaniKaniToken { get; set; }
    public DateTime LastLogin { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}