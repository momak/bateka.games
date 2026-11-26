using bateka.games.Domain.Entities;

namespace bateka.games.Services.Abstractions;

public interface IPlayerRepository
{
    /// <summary>The player (tracked), or null when they never marked anything.</summary>
    Task<Player?> GetAsync(string userId, CancellationToken ct);

    void Add(Player player);

    Task SaveChangesAsync(CancellationToken ct);
}
