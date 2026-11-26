namespace bateka.games.Engine;

/// <summary>How hard the computer plays (Tic Tac Toe and Connect Four).</summary>
public enum Difficulty
{
    /// <summary>Random moves.</summary>
    Easy,

    /// <summary>Sometimes smart: wins or blocks when it sees it, otherwise random.</summary>
    Medium,

    /// <summary>Searches ahead (minimax).</summary>
    Hard
}
