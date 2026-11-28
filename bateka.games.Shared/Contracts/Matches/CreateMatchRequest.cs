using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Contracts.Matches;

/// <summary>A new online match: its invite link is the match's id.</summary>
public sealed class CreateMatchRequest
{
    /// <summary>Tic Tac Toe or Connect Four (<c>MatchRules.Supports</c>).</summary>
    public GameKind Game { get; init; }

    /// <summary>Time per move: one of <c>MatchLimits.MoveSecondsOptions</c>.</summary>
    public int MoveSeconds { get; init; }

    /// <summary>Invite this player only (a username from GET api/players); null = anyone with the link.</summary>
    public string? Opponent { get; init; }
}
