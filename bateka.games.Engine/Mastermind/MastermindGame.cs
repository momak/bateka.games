namespace bateka.games.Engine.Mastermind;

/// <summary>
/// One game: the computer hides a code, you crack it within <see cref="MastermindSetup.Guesses"/>
/// guesses. Colours are numbers 0…Colours-1; the page picks how they look. The page owns the clock
/// (time is shown and kept for the leaderboard, not a rule), like Memory Match.
/// </summary>
public sealed class MastermindGame(Random random)
{
    private readonly List<MastermindRow> _rows = [];
    private int[] _code = [];

    public MastermindSetup Setup { get; private set; } = MastermindRules.Classic;

    public IReadOnlyList<MastermindRow> Rows => _rows;

    public bool IsStarted { get; private set; }

    public bool IsSolved { get; private set; }

    public bool IsOver => IsSolved || (IsStarted && _rows.Count >= Setup.Guesses);

    /// <summary>Guesses used so far (the leaderboard's number when solved).</summary>
    public int GuessesUsed => _rows.Count;

    public int GuessesLeft => Setup.Guesses - _rows.Count;

    /// <summary>The code — only once the game is over, so the page can't show it early by mistake.</summary>
    public IReadOnlyList<int>? RevealedCode => IsOver ? _code : null;

    public void Start(MastermindSetup setup)
    {
        if (!MastermindRules.IsValidSetup(setup))
        {
            throw new ArgumentOutOfRangeException(nameof(setup), setup, "Not a playable board.");
        }

        Setup = setup;
        _code = NewCode(setup);
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
        ? Enumerable.Range(0, setup.Columns).Select(_ => random.Next(setup.Colours)).ToArray()
        : Enumerable.Range(0, setup.Colours).OrderBy(_ => random.Next()).Take(setup.Columns).ToArray();
}
