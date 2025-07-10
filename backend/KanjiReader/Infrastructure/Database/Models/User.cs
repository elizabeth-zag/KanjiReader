using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Infrastructure.Database.Models;

public class User : IdentityUser
{
    public string? WaniKaniToken { get; set; }
    public DateTime LastLogin { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}