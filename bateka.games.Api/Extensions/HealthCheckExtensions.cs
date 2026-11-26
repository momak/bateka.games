using bateka.games.Data;
using bateka.games.Api.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace bateka.games.Api.Extensions;

public static class HealthCheckExtensions
{
    /// <summary>Registers the checks behind /health/ready — the app's database.</summary>
    public static IServiceCollection AddGamesHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck<GamesDbContext>>(HealthRoutes.DatabaseCheck, tags: [HealthRoutes.ReadyTag]);

        return services;
    }

    /// <summary>
    /// Answers /health/live and /health/ready. Call it first in the pipeline, so the health
    /// endpoints need no API key or user, and aren't request-logged every minute.
    /// </summary>
    public static IApplicationBuilder UseGamesHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks(HealthRoutes.Live, new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthResponseWriter.WriteAsync
        });

        app.UseHealthChecks(HealthRoutes.Ready, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(HealthRoutes.ReadyTag),
            ResponseWriter = HealthResponseWriter.WriteAsync
        });

        return app;
    }
}
