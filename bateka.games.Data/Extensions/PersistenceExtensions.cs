using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace bateka.games.Data.Extensions;

public static class PersistenceExtensions
{
    /// <summary>Postgres (EF Core) — the app's own database batekaGames on the shared instance.</summary>
    public static IServiceCollection AddGamesPersistence(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<GamesDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }

    /// <summary>
    /// Applies pending migrations, creating the database on first run (the shared Postgres login
    /// may create databases). Same as fit, Bateka.AI, BillTracker and Bateka.Healthchecks.
    /// </summary>
    public static async Task MigrateGamesDatabaseAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
        await db.Database.MigrateAsync(ct);
    }
}
