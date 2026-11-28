using bateka.games.Domain.Entities;

namespace bateka.games.Services.Abstractions;

public interface IMatchRepository
{
    /// <summary>The match (tracked), or null.</summary>
    Task<Match?> GetAsync(Guid id, CancellationToken ct);

    /// <summary>The user's waiting and playing matches, plus their <paramref name="recentFinished"/> latest ended ones (tracked).</summary>
    Task<IReadOnlyList<Match>> ListForUserAsync(string userId, int recentFinished, CancellationToken ct);

    void Add(Match match);

    /// <summary>False when someone else changed the match since it was read (the save is dropped).</summary>
    Task<bool> TrySaveChangesAsync(CancellationToken ct);
}
