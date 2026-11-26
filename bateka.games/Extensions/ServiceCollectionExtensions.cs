using bateka.games.Configuration;
using bateka.games.Interop;
using bateka.games.Presentation;
using bateka.games.Services;
using Bateka.UI.Extensions;
using Bateka.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace bateka.games.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The typed API clients. The base address comes from <see cref="ApiClientOptions"/>, falling
    /// back to the origin this app was served from — the deployed case, where Bateka.BFF proxies
    /// /api/* same-origin. Every client goes through Bateka.UI's <see cref="SessionExpiredHandler"/>
    /// (registered by <see cref="AddGamesUi"/>). Same as fit.
    /// </summary>
    public static IServiceCollection AddGamesApiClients(
        this IServiceCollection services, IConfiguration configuration, string hostBaseAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hostBaseAddress);

        services
            .AddOptions<ApiClientOptions>()
            .Bind(configuration.GetSection(ApiClientOptions.SectionName));

        services.AddApiClient<ResultsApiClient>(hostBaseAddress);
        services.AddApiClient<StatsApiClient>(hostBaseAddress);

        return services;
    }

    /// <summary>
    /// The shared Bateka UI (the shell, busy tracker, theme / page / update JS helpers, the 401
    /// handler, the BFF session client, MudBlazor's services) plus games' own UI services.
    /// </summary>
    public static IServiceCollection AddGamesUi(this IServiceCollection services, string hostBaseAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hostBaseAddress);

        services.AddBatekaUi(options =>
        {
            options.AppName = AppIdentity.Name;
            options.UpdateSavedNote = AppIdentity.UpdateSavedNote;
            options.HostBaseAddress = hostBaseAddress;
        });

        // Snake's canvas (the snake.js module), loaded once per session
        services.AddScoped<SnakeCanvas>();

        return services;
    }

    private static void AddApiClient<TClient>(this IServiceCollection services, string hostBaseAddress)
        where TClient : class =>
        services.AddHttpClient<TClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApiClientOptions>>().Value;
                client.BaseAddress = new Uri(options.ResolveBaseAddress(hostBaseAddress));
            })
            .AddHttpMessageHandler<SessionExpiredHandler>();
}
