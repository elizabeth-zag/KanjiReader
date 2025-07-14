using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class KanjiReaderDbContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<EventDb> Events { get; set; }
    public DbSet<ProcessingResultDb> ProcessingResults { get; set; }

    public KanjiReaderDbContext(DbContextOptions<KanjiReaderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<EventDb>().ToTable("Events");
        modelBuilder.Entity<ProcessingResultDb>().ToTable("ProcessingResults");
        
        modelBuilder.Entity<EventDb>()
            .Property(a => a.Data)
            .HasColumnType("jsonb");
    }
}
