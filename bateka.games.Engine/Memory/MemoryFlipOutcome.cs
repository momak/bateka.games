namespace bateka.games.Engine.Memory;

/// <summary>What tapping a card did.</summary>
public enum MemoryFlipOutcome
{
    /// <summary>Nothing: already shown, a pair is being checked, or the game isn't running.</summary>
    Ignored,

    /// <summary>The first card of a pair is turned over.</summary>
    FirstCard,

    /// <summary>
    /// The second card is turned over and counted as a move. The page shows both for a moment,
    /// then calls <see cref="MemoryGame.ResolvePair"/>.
    /// </summary>
    SecondCard
}
