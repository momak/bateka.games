using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;

namespace bateka.games.Services.Matches;

/// <summary>Who sits where in a match: the seats, the turn, the other player.</summary>
public static class MatchPlayers
{
    public static bool IsPlayer(Match match, string userId) =>
        match.CreatorId == userId || match.OpponentId == userId;

    /// <summary>
    /// A waiting match is the invite: anyone with the link may see it — or, for a direct invite,
    /// only the invited player. After that only its players.
    /// </summary>
    public static bool CanSee(Match match, string userId) =>
        IsPlayer(match, userId)
        || (match.Status == MatchStatus.Waiting && (match.InvitedId is null || match.InvitedId == userId));

    /// <summary>The other seat's user; null while nobody has joined.</summary>
    public static string? Other(Match match, string userId) =>
        match.CreatorId == userId ? match.OpponentId : match.CreatorId;

    /// <summary>The user in a seat; null before the opponent joins.</summary>
    public static string? InSeat(Match match, MatchSide side) =>
        side == MatchSide.First ? match.FirstPlayerId : SecondPlayer(match);

    /// <summary>Whose turn it is; null unless playing.</summary>
    public static string? ToMove(Match match) =>
        match.Status == MatchStatus.Playing ? InSeat(match, MatchRules.SideToMove(match.Moves.Count)) : null;

    private static string? SecondPlayer(Match match) =>
        match.FirstPlayerId is null ? null : Other(match, match.FirstPlayerId);
}
