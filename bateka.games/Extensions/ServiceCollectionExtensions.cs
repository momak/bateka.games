using bateka.games.Interop;
using bateka.games.Presentation;
using bateka.games.Stats;
using Bateka.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace bateka.games.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The shared Bateka UI (the shell, busy tracker, theme / page / update JS helpers, the 401
    /// handler, the BFF session client, MudBlazor's services) plus games' own UI services. The typed
    /// API clients join in the backend increment (AddGamesApiClients).
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

        // stats on this device, until the backend increment moves them to the API
        services.AddScoped<LocalStatsStore>();

        return services;
    }
}
