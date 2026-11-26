using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace bateka.games.Api.HealthChecks;

/// <summary>Healthy when the app's own database accepts a connection.</summary>
public sealed class DatabaseHealthCheck<TContext>(TContext db) : IHealthCheck where TContext : DbContext
{
    private const string Unreachable = "The database can't be reached.";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await db.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy(Unreachable);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(Unreachable, ex);
        }
    }
}
