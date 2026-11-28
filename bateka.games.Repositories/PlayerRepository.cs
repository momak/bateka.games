using bateka.games.Data;
using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Repositories;

public sealed class PlayerRepository(GamesDbContext db) : IPlayerRepository
{
    public Task<Player?> GetAsync(string userId, CancellationToken ct) =>
        db.Players.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task<IReadOnlyList<string>> ListKnownAsync(CancellationToken ct)
    {
        var fromResults = db.GameResults.Select(x => x.UserId);
        var fromPlayers = db.Players.Select(x => x.UserId);
        var creators = db.Matches.Select(x => x.CreatorId);
        var opponents = db.Matches.Where(x => x.OpponentId != null).Select(x => x.OpponentId!);

        // one SQL UNION — duplicates go away in the database
        return await fromResults.Union(fromPlayers).Union(creators).Union(opponents)
            .OrderBy(x => x)
            .ToListAsync(ct);
    }

    public void Add(Player player) => db.Players.Add(player);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
