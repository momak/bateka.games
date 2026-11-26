using bateka.games.Data;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Repositories;

public sealed class GamesStatsRepository(GamesDbContext db) : IGamesStatsRepository
{
    public async Task<GamesCounts> GetCountsAsync(DateTime dayAgo, DateTime activeSince, CancellationToken ct) =>
        new(Games24h: await db.GameResults.CountAsync(r => r.PlayedAt >= dayAgo, ct),
            ActivePlayers: await db.GameResults.Where(r => r.PlayedAt >= activeSince).Select(r => r.UserId).Distinct().CountAsync(ct),
            TotalGames: await db.GameResults.CountAsync(ct));
}
