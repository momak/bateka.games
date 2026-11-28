namespace bateka.games.Engine;

/// <summary>
/// How hard the computer plays (Tic Tac Toe and Connect Four), or how hard its code is to crack
/// (Mastermind: see MastermindRules — Easy no repeats, Medium the classic game, Hard 5 pegs of 8 colours).
/// </summary>
public enum Difficulty
{
    /// <summary>Random moves.</summary>
    Easy,

    /// <summary>Sometimes smart: wins or blocks when it sees it, otherwise random.</summary>
    Medium,

    /// <summary>Searches ahead (minimax).</summary>
    Hard
}
