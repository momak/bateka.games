using bateka.games.Api.Configuration;
using Microsoft.Extensions.Options;

namespace bateka.games.Api.Middleware;

/// <summary>
/// Checks the API key header Bateka.BFF adds on requests it proxies here. Without it, anything on
/// bateka-net could call the API directly — and pretend to be any user via X-Bateka-User.
/// </summary>
public sealed class ApiKeyMiddleware
{
    /// <summary>Header Bateka.BFF sets. Must match the name the BFF sends.</summary>
    public const string HeaderName = "X-Api-Key";

    private const string Rejected = "Missing or invalid API key.";

    private readonly RequestDelegate _next;
    private readonly ApiSecurityOptions _options;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiSecurityOptions> options, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;

        // Logged once, when the pipeline is built, so an open API is visible in the startup log.
        if (_options.IsEnabled)
            logger.LogInformation("API key check is enabled.");
        else
            logger.LogWarning("API key check is disabled — {Section}:{Key} is not configured. Fine locally; behind Bateka.BFF it means the key never arrived.",
                ApiSecurityOptions.SectionName, nameof(ApiSecurityOptions.ApiKey));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.IsEnabled)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided)
            || !string.Equals(provided, _options.ApiKey, StringComparison.Ordinal))
        {
            // 403, not 401: the person is logged in fine — the BFF→API key is wrong (a deployment problem).
            _logger.LogWarning("Rejected {Method} {Path}: {Header} {State}. Bateka.BFF's ApiKeys:games must equal {Section}:{Key}.",
                context.Request.Method, context.Request.Path, HeaderName,
                provided.Count == 0 ? "missing" : "does not match",
                ApiSecurityOptions.SectionName, nameof(ApiSecurityOptions.ApiKey));

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(Rejected);
            return;
        }

        await _next(context);
    }
}
