namespace bateka.games.Shared.Contracts.Matches;

/// <summary>How a match ended. Null while it's still waiting or being played.</summary>
public enum MatchEndReason
{
    Won,
    Draw,
    Resigned,

    /// <summary>The player to move let the clock run out, and lost.</summary>
    TimedOut,

    /// <summary>Its creator took the invite back before anyone joined.</summary>
    Withdrawn,

    /// <summary>Nobody joined in time.</summary>
    InviteExpired
}
