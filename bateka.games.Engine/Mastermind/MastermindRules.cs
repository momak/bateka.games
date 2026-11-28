namespace bateka.games.Engine.Mastermind;

/// <summary>
/// What a board may be. You pick it before playing (like Memory Match's pairs): columns, colours,
/// repeats on or off, and how many guesses you get. Classic Mastermind is 4 columns, 6 colours,
/// repeats allowed, 10 guesses.
/// </summary>
public static class MastermindRules
{
    public const int MinColumns = 3;
    public const int MaxColumns = 6;

    public const int MinColours = 4;
    public const int MaxColours = 8;

    public const int MinGuesses = 6;
    public const int MaxGuesses = 12;

    public static readonly MastermindSetup Classic = new(Columns: 4, Colours: 6, AllowRepeats: true, Guesses: 10);

    /// <summary>
    /// A board that can be played: every number in range, and without repeats there must be at least
    /// as many colours as columns (a code can't be 6 different colours out of 4).
    /// </summary>
    public static bool IsValidSetup(MastermindSetup setup) =>
        setup.Columns is >= MinColumns and <= MaxColumns
        && setup.Colours is >= MinColours and <= MaxColours
        && setup.Guesses is >= MinGuesses and <= MaxGuesses
        && (setup.AllowRepeats || setup.Colours >= setup.Columns);

    /// <summary>A guess (or a code) this board accepts: the right length, known colours, repeats only if allowed.</summary>
    public static bool IsValid(MastermindSetup setup, IReadOnlyList<int> pegs) =>
        pegs.Count == setup.Columns
        && pegs.All(colour => colour >= 0 && colour < setup.Colours)
        && (setup.AllowRepeats || pegs.Distinct().Count() == pegs.Count);
}
