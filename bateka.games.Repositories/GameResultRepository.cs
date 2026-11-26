using bateka.games.Data;
using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Boards;
using bateka.games.Shared.Contracts.Games;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Repositories;

public sealed class GameResultRepository(GamesDbContext db) : IGameResultRepository
{
    public async Task<IReadOnlyList<GameResult>> ListForUserAsync(string userId, CancellationToken ct) =>
        await db.GameResults.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.PlayedAt)
            .ToListAsync(ct);

    // uses the (Game, Difficulty) / (Game, Pairs) indexes
    public async Task<IReadOnlyList<GameResult>> ListForBoardAsync(BoardKey board, CancellationToken ct) =>
        await db.GameResults.AsNoTracking()
            .Where(x => x.Game == board.Game && x.Difficulty == board.Difficulty && x.Pairs == board.Pairs)
            .OrderBy(x => x.PlayedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<GameResult>> ListForGamesAsync(IReadOnlyCollection<GameKind> games, CancellationToken ct) =>
        await db.GameResults.AsNoTracking()
            .Where(x => games.Contains(x.Game))
            .OrderBy(x => x.PlayedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<GameResult>> ListAllAsync(CancellationToken ct) =>
        await db.GameResults.AsNoTracking()
            .OrderBy(x => x.PlayedAt)
            .ToListAsync(ct);

    public void Add(GameResult result) => db.GameResults.Add(result);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
