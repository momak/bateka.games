namespace bateka.games.Configuration;

/// <summary>
/// Where the API lives. Bound from the "ApiClient" section of wwwroot/appsettings*.json. Same as fit.
/// </summary>
public sealed class ApiClientOptions
{
    public const string SectionName = "ApiClient";

    /// <summary>
    /// Absolute base address of the API, or empty to call the origin serving this app.
    /// Empty is the deployed case — Bateka.BFF proxies /api/* from the same origin
    /// (games.bateka.dpdns.org/api/** → games-api). appsettings.Development.json points it at the
    /// API's dev port so a local run works without the BFF.
    /// </summary>
    public string? BaseAddress { get; init; }

    /// <summary>The address to actually use, given the origin this app was served from.</summary>
    public string ResolveBaseAddress(string hostBaseAddress) =>
        string.IsNullOrWhiteSpace(BaseAddress) ? hostBaseAddress : BaseAddress;
}
