using bateka.games.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Data;

/// <summary>The app's own database (batekaGames). Each entity's mapping is in TypeConfigurations/.</summary>
public class GamesDbContext(DbContextOptions<GamesDbContext> options) : DbContext(options)
{
    public DbSet<GameResult> GameResults => Set<GameResult>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesDbContext).Assembly);
}
