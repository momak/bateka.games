using bateka.games.Services.Models;

namespace bateka.games.Services.Abstractions;

/// <summary>Counts for Bateka.Healthchecks — nothing about who scored what.</summary>
public interface IGamesStatsRepository
{
    Task<GamesCounts> GetCountsAsync(DateTime dayAgo, DateTime activeSince, CancellationToken ct);
}
