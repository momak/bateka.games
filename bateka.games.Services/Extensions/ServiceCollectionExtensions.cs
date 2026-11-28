using bateka.games.Services.Achievements;
using bateka.games.Services.GameResults;
using bateka.games.Services.Leaderboards;
using bateka.games.Services.Matches;
using bateka.games.Services.Notes;
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
        services.AddScoped<LeaderboardService>();
        services.AddScoped<NoteService>();
        services.AddScoped<AchievementService>();
        services.AddScoped<MatchService>();
        return services;
    }
}
