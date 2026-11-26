namespace bateka.games.Api.Configuration;

/// <summary>Bound from "Cors" — the Blazor client origins allowed to call the API (local development only).</summary>
public sealed class CorsSettings
{
    public const string SectionName = "Cors";
    public const string PolicyName = "BlazorPolicy";

    /// <summary>Exact origins (scheme + host + port), e.g. "https://localhost:7216".</summary>
    public string[] AllowedOrigins { get; init; } = [];
}
