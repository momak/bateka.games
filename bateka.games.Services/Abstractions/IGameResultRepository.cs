using bateka.games.Domain.Entities;

namespace bateka.games.Services.Abstractions;

/// <summary>Storage of finished games.</summary>
public interface IGameResultRepository
{
    /// <summary>All of one user's results, oldest first. Not tracked.</summary>
    Task<IReadOnlyList<GameResult>> ListForUserAsync(string userId, CancellationToken ct);

    void Add(GameResult result);

    Task SaveChangesAsync(CancellationToken ct);
}
