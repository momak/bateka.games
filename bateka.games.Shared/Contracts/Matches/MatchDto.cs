using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Contracts.Matches;

/// <summary>
/// A match as both players (and, while it's waiting, anyone with the link) see it. The same object
/// goes to both players, so it has no "you": the client compares the names with its own user.
/// The board itself is <see cref="Moves"/> replayed with <c>MatchRules</c>.
/// </summary>
public sealed record MatchDto
{
    public Guid Id { get; init; }
    public GameKind Game { get; init; }
    public MatchStatus Status { get; init; }

    /// <summary>Who sent the invite.</summary>
    public string Creator { get; init; } = string.Empty;

    /// <summary>Who joined; null while waiting.</summary>
    public string? Opponent { get; init; }

    /// <summary>Who moves first (X / Red), picked at random when the opponent joins.</summary>
    public string? FirstPlayer { get; init; }

    /// <summary>Whose turn it is; null unless playing.</summary>
    public string? PlayerToMove { get; init; }

    /// <summary>Every move so far, in order: a square (0–8) or a column (0–6).</summary>
    public IReadOnlyList<int> Moves { get; init; } = [];

    /// <summary>Time for each move (one of <c>MatchLimits.MoveSecondsOptions</c>).</summary>
    public int MoveSeconds { get; init; }

    /// <summary>When the player to move runs out of time (UTC); null unless playing.</summary>
    public DateTime? MoveDeadline { get; init; }

    public string? Winner { get; init; }
    public MatchEndReason? EndReason { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? FinishedAt { get; init; }
}
