using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Matches;

namespace bateka.games.Domain.Entities;

/// <summary>
/// An online match between two signed-in users. Only the moves are stored; the board is worked out
/// from them (MatchRules), so the stored state can't disagree with the rules. Not a GameResult:
/// online matches don't count on the boards against the computer.
/// </summary>
public class Match
{
    /// <summary>Also the invite: the link is /match/{Id}.</summary>
    public Guid Id { get; set; }

    public GameKind Game { get; set; }
    public MatchStatus Status { get; set; }

    /// <summary>Username from Bateka.BFF (X-Bateka-User) of whoever created the invite.</summary>
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>Who joined; null while waiting.</summary>
    public string? OpponentId { get; set; }

    /// <summary>X / Red — picked at random when the opponent joins.</summary>
    public string? FirstPlayerId { get; set; }

    /// <summary>Every move in order. Replace the list when adding one (EF compares lists by value).</summary>
    public List<int> Moves { get; set; } = [];

    public int MoveSeconds { get; set; }

    /// <summary>When the player to move runs out of time; null unless playing.</summary>
    public DateTime? MoveDeadline { get; set; }

    public string? WinnerId { get; set; }
    public MatchEndReason? EndReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    /// <summary>Postgres xmin: two moves saved at the same moment → the second one fails instead of overwriting.</summary>
    public uint Version { get; set; }
}
