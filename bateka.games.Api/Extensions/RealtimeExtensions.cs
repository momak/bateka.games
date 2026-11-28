using bateka.games.Api.Hubs;
using bateka.games.Api.Matches;
using bateka.games.Services.Abstractions;
using bateka.games.Shared.Json;
using bateka.games.Shared.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace bateka.games.Api.Extensions;

public static class RealtimeExtensions
{
    /// <summary>
    /// SignalR for online matches (same JSON shape as the controllers), the notifier MatchService
    /// pushes through, and the match clock. One games-api instance, so no backplane.
    /// </summary>
    public static IServiceCollection AddGamesRealtime(this IServiceCollection services)
    {
        services.AddSignalR()
            .AddJsonProtocol(options => GamesJson.Configure(options.PayloadSerializerOptions));

        // Clients.User(name) = every connection of that Bateka user (their phone and their PC)
        services.AddSingleton<IUserIdProvider, ForwardedUserIdProvider>();
        services.AddSingleton<IMatchNotifier, MatchHubNotifier>();
        services.AddHostedService<MatchClock>();

        return services;
    }

    /// <summary>Maps the hub. After UseGamesSecurity, so every connection has the API key and a user.</summary>
    public static IEndpointRouteBuilder MapGamesHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<MatchHub>("/" + MatchHubContract.Path);
        return endpoints;
    }
}
