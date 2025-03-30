using Microsoft.EntityFrameworkCore;

namespace TokenCardCare.Server.Entity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Card> Cards { get; set; } = null!;
    public DbSet<FoundCard> FoundCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>().HasIndex(entity => entity.Sno);
        modelBuilder.Entity<Card>().HasIndex(entity => entity.Type);
    }
}
