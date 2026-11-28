using bateka.games.Services.Matches;

namespace bateka.games.Api.Matches;

/// <summary>
/// Runs <see cref="MatchClockService"/> every few seconds: a player who lets the time run out loses
/// even if nobody opens the match again, and the winner sees it live. Reads and moves apply the same
/// rule themselves, so the interval only decides how soon the push arrives.
/// </summary>
public sealed class MatchClock(IServiceScopeFactory scopes, ILogger<MatchClock> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await TickAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // shutting down
        }
    }

    private async Task TickAsync(CancellationToken ct)
    {
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            var ended = await scope.ServiceProvider.GetRequiredService<MatchClockService>().TickAsync(ct);

            if (ended > 0) logger.LogInformation("Match clock ended {Count} match(es)", ended);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // e.g. Postgres restarting: skip this tick, try again on the next
            logger.LogWarning(ex, "Match clock tick failed");
        }
    }
}
