namespace bateka.games.Engine.Memory;

/// <summary>How a pair of turned-over cards ended.</summary>
public enum MemoryPairOutcome
{
    /// <summary>No pair was waiting.</summary>
    None,

    /// <summary>Different faces: both turned back.</summary>
    Mismatch,

    /// <summary>Same face: both stay shown.</summary>
    Match,

    /// <summary>Same face, and it was the last pair: the game is won.</summary>
    Won
}
