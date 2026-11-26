using bateka.games.Services.Abstractions;

namespace bateka.games.Api.Identity;

/// <summary>The user ForwardedUserMiddleware resolved for this request (X-Bateka-User from Bateka.BFF).</summary>
public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string UserId =>
        (accessor.HttpContext ?? throw new InvalidOperationException("No HTTP request — ICurrentUser only works inside a request."))
        .GetRequiredUserId();
}
