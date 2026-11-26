using System.Text.Json;
using System.Text.Json.Serialization;
using bateka.games.Api.Stats;

namespace bateka.games.Api.Extensions;

public static class InternalStatsExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>The provider behind /internal/stats (scoped: it uses the app's DbContext).</summary>
    public static IServiceCollection AddGamesInternalStats(this IServiceCollection services) =>
        services.AddScoped<GamesStatsProvider>();

    /// <summary>
    /// Answers <see cref="InternalStatsRoutes.Stats"/> with this app's own numbers. Call it next to
    /// the health endpoints, before the API key / user middleware.
    /// </summary>
    public static IApplicationBuilder UseGamesInternalStats(this IApplicationBuilder app) =>
        app.Map(InternalStatsRoutes.Stats, branch => branch.Run(async context =>
        {
            var provider = context.RequestServices.GetRequiredService<GamesStatsProvider>();
            var stats = await provider.GetAsync(context.RequestAborted);
            await context.Response.WriteAsJsonAsync(
                new InternalStatsResponse(DateTime.UtcNow, stats), JsonOptions, context.RequestAborted);
        }));
}
