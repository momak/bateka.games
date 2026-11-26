using bateka.games.Domain.Entities;
using bateka.games.Services.Boards;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Services.Abstractions;

/// <summary>Storage of finished games. All lists are not tracked.</summary>
public interface IGameResultRepository
{
    /// <summary>All of one user's results, oldest first.</summary>
    Task<IReadOnlyList<GameResult>> ListForUserAsync(string userId, CancellationToken ct);

    /// <summary>Everyone's results on one board.</summary>
    Task<IReadOnlyList<GameResult>> ListForBoardAsync(BoardKey board, CancellationToken ct);

    /// <summary>Everyone's results in the given games (the notes look at every board you're on).</summary>
    Task<IReadOnlyList<GameResult>> ListForGamesAsync(IReadOnlyCollection<GameKind> games, CancellationToken ct);

    /// <summary>Everyone's results (achievements: who else unlocked what). Fine at family scale.</summary>
    Task<IReadOnlyList<GameResult>> ListAllAsync(CancellationToken ct);

    void Add(GameResult result);

    Task SaveChangesAsync(CancellationToken ct);
}
