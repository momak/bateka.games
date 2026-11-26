namespace bateka.games.Api.Configuration;

/// <summary>
/// Who the caller is. Behind Bateka.BFF that's the X-Bateka-User header; running locally without
/// the BFF there's no header, so <see cref="DevelopmentUser"/> stands in. Bound from "Identity" —
/// set it in appsettings.Development.json only, never in Docker.
/// </summary>
public sealed class ForwardedIdentityOptions
{
    public const string SectionName = "Identity";

    /// <summary>Header Bateka.BFF sets (its ForwardedIdentity.UserHeader). URL-encoded username.</summary>
    public const string UserHeader = "X-Bateka-User";

    /// <summary>Used when the header is missing. Empty = the request is refused (401).</summary>
    public string? DevelopmentUser { get; init; }
}
