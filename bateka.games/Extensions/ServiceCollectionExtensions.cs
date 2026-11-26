using bateka.games.Presentation;
using Bateka.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace bateka.games.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The shared Bateka UI (the shell, busy tracker, theme / page / update JS helpers, the 401
    /// handler, the BFF session client, MudBlazor's services). The typed API clients join in the
    /// backend increment (AddGamesApiClients).
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

        return services;
    }
}
