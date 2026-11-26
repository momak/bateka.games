namespace bateka.games.Services.Abstractions;

/// <summary>
/// Who is making the current request — the username Bateka.BFF forwards (X-Bateka-User). The
/// services use it to scope every query to the user's own data, so a controller can't forget to
/// pass it. Same as fit; implemented in the Api.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The username. Throws when the request carries none (the Api refuses those earlier).</summary>
    string UserId { get; }
}
