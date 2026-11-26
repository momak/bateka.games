using bateka.games.Services.GameResults;
using Microsoft.Extensions.DependencyInjection;

namespace bateka.games.Services.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The service layer. Needs, from the Api: ICurrentUser, the repositories
    /// (bateka.games.Repositories) and TimeProvider.
    /// </summary>
    public static IServiceCollection AddGamesServices(this IServiceCollection services)
    {
        services.AddScoped<GameResultService>();
        return services;
    }
}
