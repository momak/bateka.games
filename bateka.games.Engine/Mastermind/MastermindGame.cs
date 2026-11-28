namespace bateka.games.Engine.Mastermind;

/// <summary>
/// One game: the computer hides a code, you crack it in at most <see cref="MastermindSetup.MaxGuesses"/>
/// guesses. Colours are numbers 0…Colours-1; the page picks how they look. The page owns the clock
/// (time is shown and kept for the leaderboard, not a rule), like Memory Match.
/// </summary>
public sealed class MastermindGame(Random random)
{
    private readonly List<MastermindRow> _rows = [];
    private int[] _code = [];

    public MastermindSetup Setup { get; private set; } = MastermindRules.Medium;

    public Difficulty Difficulty { get; private set; } = Difficulty.Medium;

    public IReadOnlyList<MastermindRow> Rows => _rows;

    public bool IsStarted { get; private set; }

    public bool IsSolved { get; private set; }

    public bool IsOver => IsSolved || (IsStarted && _rows.Count >= Setup.MaxGuesses);

    /// <summary>Guesses used so far (the leaderboard's number when solved).</summary>
    public int Guesses => _rows.Count;

    public int GuessesLeft => Setup.MaxGuesses - _rows.Count;

    /// <summary>The code — only once the game is over, so the page can't show it early by mistake.</summary>
    public IReadOnlyList<int>? RevealedCode => IsOver ? _code : null;

    public void Start(Difficulty difficulty)
    {
        Difficulty = difficulty;
        Setup = MastermindRules.For(difficulty);
        _code = NewCode(Setup);
        _rows.Clear();
        IsSolved = false;
        IsStarted = true;
    }

    public MastermindGuessOutcome Guess(IReadOnlyList<int> guess)
    {
        if (!IsStarted || IsOver || !MastermindRules.IsValid(Setup, guess)) return MastermindGuessOutcome.Invalid;

        var feedback = MastermindScorer.Score(_code, guess);
        _rows.Add(new MastermindRow([.. guess], feedback));

        if (feedback.Cracks(Setup))
        {
            IsSolved = true;
            return MastermindGuessOutcome.Solved;
        }

        return IsOver ? MastermindGuessOutcome.OutOfGuesses : MastermindGuessOutcome.Scored;
    }

    private int[] NewCode(MastermindSetup setup) => setup.AllowRepeats
        ? Enumerable.Range(0, setup.Pegs).Select(_ => random.Next(setup.Colours)).ToArray()
        : Enumerable.Range(0, setup.Colours).OrderBy(_ => random.Next()).Take(setup.Pegs).ToArray();
}
