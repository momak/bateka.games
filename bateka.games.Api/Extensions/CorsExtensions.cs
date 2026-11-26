using bateka.games.Api.Configuration;

namespace bateka.games.Api.Extensions;

public static class CorsExtensions
{
    /// <summary>
    /// Registers the Blazor CORS policy from Cors:AllowedOrigins. Only matters locally — behind
    /// Bateka.BFF the client and the API share one origin.
    /// </summary>
    public static IServiceCollection AddGamesCors(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>() ?? new CorsSettings();

        services.AddCors(options => options.AddPolicy(CorsSettings.PolicyName, policy => policy
            .WithOrigins(settings.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        return services;
    }

    public static IApplicationBuilder UseGamesCors(this IApplicationBuilder app) =>
        app.UseCors(CorsSettings.PolicyName);
}
