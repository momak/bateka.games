using bateka.games.Data;
using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Repositories;

public sealed class GameResultRepository(GamesDbContext db) : IGameResultRepository
{
    public async Task<IReadOnlyList<GameResult>> ListForUserAsync(string userId, CancellationToken ct) =>
        await db.GameResults.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.PlayedAt)
            .ToListAsync(ct);

    public void Add(GameResult result) => db.GameResults.Add(result);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
