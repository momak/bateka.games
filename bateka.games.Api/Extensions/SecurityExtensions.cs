using bateka.games.Api.Configuration;
using bateka.games.Api.Identity;
using bateka.games.Api.Middleware;
using bateka.games.Services.Abstractions;

namespace bateka.games.Api.Extensions;

public static class SecurityExtensions
{
    /// <summary>API key (from Bateka.BFF), the forwarded user (ICurrentUser); refuses a key that's set but too short.</summary>
    public static IServiceCollection AddGamesSecurity(this IServiceCollection services)
    {
        services.AddOptions<ApiSecurityOptions>()
            .BindConfiguration(ApiSecurityOptions.SectionName)
            .Validate(options => !options.IsEnabled || options.ApiKey!.Length >= ApiSecurityOptions.MinimumKeyLength,
                $"{ApiSecurityOptions.SectionName}:{nameof(ApiSecurityOptions.ApiKey)} must be at least {ApiSecurityOptions.MinimumKeyLength} characters.")
            .ValidateOnStart();

        services.AddOptions<ForwardedIdentityOptions>()
            .BindConfiguration(ForwardedIdentityOptions.SectionName);

        services.AddSingleton<ForwardedUserResolver>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();

        return services;
    }

    /// <summary>
    /// API key first (proves the request came through Bateka.BFF), then the user. Call it after the
    /// health / internal endpoints and before MapControllers.
    /// </summary>
    public static IApplicationBuilder UseGamesSecurity(this IApplicationBuilder app) =>
        app.UseMiddleware<ApiKeyMiddleware>()
           .UseMiddleware<ForwardedUserMiddleware>();
}
