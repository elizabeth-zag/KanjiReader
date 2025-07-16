using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KanjiReader.Infrastructure.Database.DbContext;

public class KanjiReaderDbContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserKanji> UserKanji { get; set; }
    public DbSet<KanjiDb> Kanji { get; set; }
    public DbSet<EventDb> Events { get; set; }
    public DbSet<ProcessingResultDb> ProcessingResults { get; set; }

    public KanjiReaderDbContext(DbContextOptions<KanjiReaderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .Entity<UserKanji>()
            .HasKey(uk => new { uk.KanjiId, uk.UserId});
        
        modelBuilder.Entity<EventDb>().ToTable("Events");
        modelBuilder.Entity<ProcessingResultDb>().ToTable("ProcessingResults");
        
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
        
        modelBuilder.Entity<EventDb>()
            .Property(a => a.Data)
            .HasColumnType("jsonb");
    }
}
