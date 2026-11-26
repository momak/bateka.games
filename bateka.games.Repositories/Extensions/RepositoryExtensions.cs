using bateka.games.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace bateka.games.Repositories.Extensions;

public static class RepositoryExtensions
{
    /// <summary>The EF Core repositories (scoped — they share the request's GamesDbContext). Needs AddGamesPersistence.</summary>
    public static IServiceCollection AddGamesRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameResultRepository, GameResultRepository>();
        services.AddScoped<IGamesStatsRepository, GamesStatsRepository>();
        return services;
    }
}
