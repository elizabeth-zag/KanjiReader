using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.DbContext;

public class KanjiReaderDbContext(DbContextOptions<KanjiReaderDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserKanji> UserKanji { get; set; }
    public DbSet<Kanji> Kanji { get; set; }
    public DbSet<ProcessingResult> ProcessingResults { get; set; }
    public DbSet<UserGenerationState> UserGenerationStates { get; set; }
    public DbSet<Text> Texts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .Entity<UserKanji>()
            .HasKey(uk => new { uk.KanjiId, uk.UserId});
        
        modelBuilder.Entity<UserGenerationState>()
            .HasIndex(e => new { e.UserId, e.SourceType })
            .IsUnique();
        
        modelBuilder.Entity<ProcessingResult>().ToTable("ProcessingResults");
        modelBuilder.Entity<UserGenerationState>().ToTable("UserGenerationStates");
        modelBuilder.Entity<Text>().ToTable("Texts");
        
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
