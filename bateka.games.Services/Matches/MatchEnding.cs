using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Matches;

namespace bateka.games.Services.Matches;

/// <summary>The two ways a match stops: finished (played) or cancelled (never played).</summary>
public static class MatchEnding
{
    /// <summary><paramref name="winnerId"/> null = draw.</summary>
    public static void Finish(Match match, string? winnerId, MatchEndReason reason, DateTime at)
    {
        match.Status = MatchStatus.Finished;
        match.WinnerId = winnerId;
        match.EndReason = reason;
        match.MoveDeadline = null;
        match.FinishedAt = at;
    }

    public static void Cancel(Match match, MatchEndReason reason, DateTime at)
    {
        match.Status = MatchStatus.Cancelled;
        match.EndReason = reason;
        match.FinishedAt = at;
    }
}
