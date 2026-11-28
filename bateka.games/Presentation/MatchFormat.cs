using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;

namespace bateka.games.Presentation;

/// <summary>How an online match reads on screen, from one player's side.</summary>
public static class MatchFormat
{
    private const string Waiting = "Waiting for someone to join";

    /// <summary>"1 min", "5 min", "1 day".</summary>
    public static string MoveTime(int seconds) => seconds switch
    {
        MatchLimits.OneDay => "1 day",
        >= 60 => $"{seconds / 60} min",
        _ => $"{seconds} s"
    };

    /// <summary>"0:42", "4:05", "23 h".</summary>
    public static string Remaining(TimeSpan left)
    {
        if (left <= TimeSpan.Zero) return "0:00";
        if (left.TotalHours >= 1) return $"{(int)Math.Ceiling(left.TotalHours)} h";
        return $"{(int)left.TotalMinutes}:{left.Seconds:00}";
    }

    public static string? OpponentOf(MatchDto match, string me) =>
        match.Creator == me ? match.Opponent : match.Creator;

    /// <summary>"X — you move first" / "O" / "Red …" for the player's own seat.</summary>
    public static string YourSeat(MatchDto match, string me)
    {
        var first = match.FirstPlayer == me;
        var mark = match.Game == GameKind.TicTacToe
            ? (first ? "X" : "O")
            : (first ? "Red" : "Yellow");
        return first ? $"You play {mark} and move first" : $"You play {mark}";
    }

    /// <summary>One line for the match's state.</summary>
    public static string Status(MatchDto match, string me) => match.Status switch
    {
        MatchStatus.Waiting when match.Creator == me => match.Invited is { } invited ? $"Waiting for {invited}" : Waiting,
        MatchStatus.Waiting => $"{match.Creator} invites you to play",
        MatchStatus.Playing => match.PlayerToMove == me ? "Your turn" : $"{match.PlayerToMove}'s turn",
        MatchStatus.Cancelled => match.EndReason == MatchEndReason.InviteExpired
            ? "Nobody joined in time"
            : "The invite was withdrawn",
        _ => Result(match, me)
    };

    private static string Result(MatchDto match, string me)
    {
        var other = OpponentOf(match, me);
        var won = match.Winner == me;

        return match.EndReason switch
        {
            MatchEndReason.Draw => "🤝 It's a draw",
            MatchEndReason.Resigned => won ? $"🎉 You win — {other} resigned" : "You resigned",
            MatchEndReason.TimedOut => won ? $"🎉 You win — {other} ran out of time" : "⏱ You ran out of time",
            _ => won ? "🎉 You win!" : $"{other} wins"
        };
    }

    /// <summary>For the "Your matches" list: what's waiting on whom.</summary>
    public static string ListLine(MatchDto match, string me) => match.Status switch
    {
        MatchStatus.Waiting when match.Creator == me => match.Invited is { } invited ? $"Invite sent to {invited}" : "Invite sent — waiting",
        MatchStatus.Waiting => $"Invite from {match.Creator}",
        MatchStatus.Playing => match.PlayerToMove == me
            ? $"Your turn vs {OpponentOf(match, me)}"
            : $"{OpponentOf(match, me)}'s turn",
        _ => Status(match, me)
    };
}
