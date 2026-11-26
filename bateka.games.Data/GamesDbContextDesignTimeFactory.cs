using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace bateka.games.Data;

/// <summary>
/// Lets `dotnet ef migrations add` build the model without starting the API (and without a real
/// connection string). Adding a migration never connects; the API migrates itself on start.
/// </summary>
public sealed class GamesDbContextDesignTimeFactory : IDesignTimeDbContextFactory<GamesDbContext>
{
    private const string DesignTimeConnection = "Host=localhost;Database=batekaGames;Username=design;Password=design";

    public GamesDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<GamesDbContext>().UseNpgsql(DesignTimeConnection).Options);
}
