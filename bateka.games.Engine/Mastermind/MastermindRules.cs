namespace bateka.games.Engine.Mastermind;

/// <summary>
/// The three boards. Medium is the classic game (4 pegs, 6 colours, repeats allowed); Easy takes the
/// repeats away, Hard adds a peg and two colours. Ten guesses on every board.
/// </summary>
public static class MastermindRules
{
    public const int MaxGuesses = 10;

    /// <summary>The most colours any board uses (the page's palette has this many).</summary>
    public const int MaxColours = 8;

    /// <summary>The longest code any board uses.</summary>
    public const int MaxPegs = 5;

    public static readonly MastermindSetup Easy = new(Pegs: 4, Colours: 6, AllowRepeats: false, MaxGuesses);
    public static readonly MastermindSetup Medium = new(Pegs: 4, Colours: 6, AllowRepeats: true, MaxGuesses);
    public static readonly MastermindSetup Hard = new(Pegs: 5, Colours: MaxColours, AllowRepeats: true, MaxGuesses);

    public static MastermindSetup For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => Easy,
        Difficulty.Medium => Medium,
        Difficulty.Hard => Hard,
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
    };

    /// <summary>A guess (or a code) this board accepts: the right length, known colours, repeats only if allowed.</summary>
    public static bool IsValid(MastermindSetup setup, IReadOnlyList<int> pegs) =>
        pegs.Count == setup.Pegs
        && pegs.All(colour => colour >= 0 && colour < setup.Colours)
        && (setup.AllowRepeats || pegs.Distinct().Count() == pegs.Count);
}
