using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class KanjiReaderDbContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }

    public KanjiReaderDbContext(DbContextOptions<KanjiReaderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .Entity<UserKanji>()
            .HasKey(uk => new { uk.KanjiId, uk.UserId});
        
        modelBuilder
            .Entity<UserKanji>()
            .HasOne(uk => uk.User)
            .WithMany(u => u.UserKanjis)
            .HasForeignKey(uk => uk.UserId);
        
        modelBuilder
            .Entity<UserKanji>()
            .HasOne(uk => uk.Kanji)
            .WithMany(u => u.UserKanjis)
            .HasForeignKey(uk => uk.KanjiId);
    }
}
