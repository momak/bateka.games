namespace bateka.games.Api.Stats;

/// <summary>
/// Numbers for Bateka.Healthchecks, polled over bateka-net. Like the health endpoints it answers
/// before the API key and user checks, and Bateka.BFF doesn't route it (only /api/** is), so it
/// can't be reached from the internet.
/// </summary>
public static class InternalStatsRoutes
{
    public const string Stats = "/internal/stats";
}
