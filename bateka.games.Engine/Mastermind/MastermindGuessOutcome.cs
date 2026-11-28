namespace bateka.games.Engine.Mastermind;

public enum MastermindGuessOutcome
{
    /// <summary>Not a guess this board takes (wrong length, unknown colour, a repeat on Easy), or the game is over.</summary>
    Invalid,

    /// <summary>Scored; keep going.</summary>
    Scored,

    /// <summary>The code is cracked.</summary>
    Solved,

    /// <summary>That was the last guess, and it wasn't the code.</summary>
    OutOfGuesses
}
