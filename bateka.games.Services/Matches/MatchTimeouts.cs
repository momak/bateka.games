using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;

namespace bateka.games.Services.Matches;

/// <summary>
/// The clock's rules: the player to move loses when their time is up; an invite nobody joined is
/// cancelled after a day. Used on every read and move (so a late clock tick never lets a late move
/// in) and by the background clock.
/// </summary>
public static class MatchTimeouts
{
    /// <summary>Ends the match if its time is up. True when it changed it.</summary>
    public static bool Apply(Match match, DateTime now)
    {
        if (match.Status == MatchStatus.Playing && match.MoveDeadline is { } deadline && now >= deadline)
        {
            var late = MatchPlayers.ToMove(match)!;
            MatchEnding.Finish(match, MatchPlayers.Other(match, late), MatchEndReason.TimedOut, deadline);
            return true;
        }

        if (match.Status == MatchStatus.Waiting && now >= InviteExpiresAt(match))
        {
            MatchEnding.Cancel(match, MatchEndReason.InviteExpired, InviteExpiresAt(match));
            return true;
        }

        return false;
    }

    public static DateTime InviteExpiresAt(Match match) => match.CreatedAt + MatchLimits.InviteLifetime;
}
