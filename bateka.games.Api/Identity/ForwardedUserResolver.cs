using bateka.games.Api.Configuration;
using Microsoft.Extensions.Options;

namespace bateka.games.Api.Identity;

/// <summary>
/// Reads the logged-in user from Bateka.BFF's X-Bateka-User header (URL-encoded username), or
/// falls back to <see cref="ForwardedIdentityOptions.DevelopmentUser"/> when running without the BFF.
/// Only trustworthy because the API key check runs first — only the BFF can reach this API.
/// </summary>
public sealed class ForwardedUserResolver(IOptions<ForwardedIdentityOptions> options)
{
    private readonly ForwardedIdentityOptions _options = options.Value;

    public bool UsesDevelopmentUser => !string.IsNullOrWhiteSpace(_options.DevelopmentUser);

    /// <summary>The user id, or null when the request carries none.</summary>
    public string? Resolve(HttpContext? context)
    {
        var header = context?.Request.Headers[ForwardedIdentityOptions.UserHeader].ToString();

        if (!string.IsNullOrWhiteSpace(header))
            return Uri.UnescapeDataString(header).Trim();

        return UsesDevelopmentUser ? _options.DevelopmentUser : null;
    }
}
