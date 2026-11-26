namespace bateka.games.Api.Configuration;

/// <summary>
/// The shared secret Bateka.BFF sends on proxied requests (ApiKeys:games there). Bound from
/// "ApiSecurity"; in Docker that arrives as ApiSecurity__ApiKey. Same as Bateka.AI.
/// It is also what makes the forwarded user header trustworthy: only the BFF knows the key.
/// </summary>
public sealed class ApiSecurityOptions
{
    public const string SectionName = "ApiSecurity";

    /// <summary>A shorter key was almost certainly truncated by the shell — fail at startup instead.</summary>
    public const int MinimumKeyLength = 16;

    /// <summary>Expected value of the API key header. Unset for local development.</summary>
    public string? ApiKey { get; init; }

    /// <summary>Whether requests are checked at all.</summary>
    public bool IsEnabled => !string.IsNullOrWhiteSpace(ApiKey);
}
