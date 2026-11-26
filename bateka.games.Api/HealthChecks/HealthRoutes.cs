namespace bateka.games.Api.HealthChecks;

/// <summary>
/// Health endpoints Bateka.Healthchecks polls over bateka-net. They sit in front of every other
/// middleware (API key, user), so they answer without credentials — and they're not routed
/// by Bateka.BFF (only /api/** is), so they're not reachable from the internet.
/// </summary>
public static class HealthRoutes
{
    /// <summary>The process is running. Checks nothing else.</summary>
    public const string Live = "/health/live";

    /// <summary>The app can do its job: its database answers.</summary>
    public const string Ready = "/health/ready";

    /// <summary>Tag on the checks that /health/ready runs.</summary>
    public const string ReadyTag = "ready";

    public const string DatabaseCheck = "database";
}
