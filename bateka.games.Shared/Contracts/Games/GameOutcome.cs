namespace bateka.games.Shared.Contracts.Games;

/// <summary>How a game ended, for the player.</summary>
public enum GameOutcome
{
    /// <summary>A game without winners or losers (Snake: it ends when you crash).</summary>
    Finished,
    Win,
    Loss,
    Draw
}
