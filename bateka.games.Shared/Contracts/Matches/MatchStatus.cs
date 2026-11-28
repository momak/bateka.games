namespace bateka.games.Shared.Contracts.Matches;

public enum MatchStatus
{
    /// <summary>Created, the invite link is out, nobody has joined yet.</summary>
    Waiting,

    Playing,

    /// <summary>Someone won, it's a draw, someone resigned or ran out of time.</summary>
    Finished,

    /// <summary>Never played: withdrawn by its creator, or the invite expired.</summary>
    Cancelled
}
