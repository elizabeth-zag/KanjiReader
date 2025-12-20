using KanjiReader.Domain.DomainObjects;
using Microsoft.AspNetCore.Identity;

namespace KanjiReader.Infrastructure.Database.Models;

public class User : IdentityUser
{
    public KanjiSourceType KanjiSourceType { get; set; }
    public string? WaniKaniToken { get; set; }
    public WaniKaniStage[]? WaniKaniStages { get; set; }
    public double? Threshold { get; set; }
    public bool HasData { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime? LastProcessingTime { get; set; }
    public DateTime? EmailConfirmExpiresAt { get; set; }
    public byte[]? EmailConfirmCodeHash { get; set; }
    public int EmailConfirmAttempts  { get; set; }
    public ICollection<UserKanji> UserKanjis { get; set; }
}