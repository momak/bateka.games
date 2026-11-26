using bateka.games.Data;
using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Repositories;

public sealed class PlayerRepository(GamesDbContext db) : IPlayerRepository
{
    public Task<Player?> GetAsync(string userId, CancellationToken ct) =>
        db.Players.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public void Add(Player player) => db.Players.Add(player);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
