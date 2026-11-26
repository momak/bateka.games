using bateka.games.Services.Abstractions;

namespace bateka.games.Api.Stats;

/// <summary>
/// bateka.games' numbers for Bateka.Healthchecks: games finished in the last day, active players and
/// all games ever. Counts only — nothing about who scored what.
/// </summary>
public sealed class GamesStatsProvider(IGamesStatsRepository stats, TimeProvider clock)
{
    private const int ActivePlayerDays = 30;

    public async Task<IReadOnlyList<InternalStat>> GetAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var counts = await stats.GetCountsAsync(now.AddDays(-1), now.AddDays(-ActivePlayerDays), ct);

        return
        [
            new("games.24h", "Games (24 h)", counts.Games24h,
                Tone: counts.Games24h > 0 ? StatTone.Good : StatTone.Neutral),
            new("players.active", $"Active players ({ActivePlayerDays} d)", counts.ActivePlayers),
            new("games.total", "Games (all time)", counts.TotalGames)
        ];
    }
}
