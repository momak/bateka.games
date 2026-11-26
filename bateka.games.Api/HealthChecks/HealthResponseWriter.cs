using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace bateka.games.Api.HealthChecks;

/// <summary>
/// Writes a health report as small JSON: overall status, and each check's status and time.
/// The status code (200 healthy, 503 unhealthy) is what Bateka.Healthchecks acts on.
/// </summary>
public static class HealthResponseWriter
{
    private const string JsonContentType = "application/json";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var body = new
        {
            Status = report.Status.ToString(),
            DurationMs = report.TotalDuration.TotalMilliseconds,
            Checks = report.Entries.Select(entry => new
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                DurationMs = entry.Value.Duration.TotalMilliseconds,
                Error = entry.Value.Exception?.Message ?? entry.Value.Description
            })
        };

        context.Response.ContentType = JsonContentType;
        return context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
