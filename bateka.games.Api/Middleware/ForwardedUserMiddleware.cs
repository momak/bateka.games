using bateka.games.Api.Configuration;
using bateka.games.Api.Identity;

namespace bateka.games.Api.Middleware;

/// <summary>
/// Refuses requests that carry no user (401) and stores the user on the request for the
/// controllers (<see cref="CurrentUserExtensions.GetRequiredUserId"/>). Every result belongs
/// to someone. Runs after the API key check, so the forwarded header can be trusted.
/// </summary>
public sealed class ForwardedUserMiddleware
{
    private const string NoUser = "No user — requests must come through Bateka.BFF.";

    private readonly RequestDelegate _next;
    private readonly ForwardedUserResolver _resolver;
    private readonly ILogger<ForwardedUserMiddleware> _logger;

    public ForwardedUserMiddleware(RequestDelegate next, ForwardedUserResolver resolver, ILogger<ForwardedUserMiddleware> logger)
    {
        _next = next;
        _resolver = resolver;
        _logger = logger;

        if (resolver.UsesDevelopmentUser)
            logger.LogWarning("Requests without {Header} run as the development user ({Section}:{Key}). Never set this behind Bateka.BFF.",
                ForwardedIdentityOptions.UserHeader, ForwardedIdentityOptions.SectionName, nameof(ForwardedIdentityOptions.DevelopmentUser));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = _resolver.Resolve(context);

        if (userId is null)
        {
            _logger.LogWarning("Rejected {Method} {Path}: {Header} missing.",
                context.Request.Method, context.Request.Path, ForwardedIdentityOptions.UserHeader);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(NoUser);
            return;
        }

        context.SetUserId(userId);
        await _next(context);
    }
}
