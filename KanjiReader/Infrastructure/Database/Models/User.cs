using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Infrastructure.Database.Models;

public class User : IdentityUser
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string WaniKaniToken { get; set; }
    public DateTime LastLogin { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}