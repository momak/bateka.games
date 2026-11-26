namespace bateka.games.Engine.Memory;

/// <summary>
/// One game of Memory Match. The page owns the clock (time is shown, not a rule) and the short
/// pause before a pair is resolved: <see cref="Flip"/> → <see cref="MemoryFlipOutcome.SecondCard"/>
/// → wait <see cref="MemoryRules.RevealMs"/> → <see cref="ResolvePair"/>.
/// </summary>
public sealed class MemoryGame(Random random)
{
    private readonly List<MemoryCard> _cards = [];
    private int? _firstIndex;
    private int? _secondIndex;

    public IReadOnlyList<MemoryCard> Cards => _cards;

    public int Pairs { get; private set; }

    /// <summary>Pairs of cards turned over (matching or not).</summary>
    public int Moves { get; private set; }

    public int MatchesFound { get; private set; }

    public bool IsStarted { get; private set; }

    public bool IsWon => IsStarted && Pairs > 0 && MatchesFound == Pairs;

    /// <summary>Two cards are shown and waiting for <see cref="ResolvePair"/>: other taps are ignored.</summary>
    public bool IsCheckingPair => _secondIndex is not null;

    public MemoryLayout Layout => MemoryRules.LayoutFor(Pairs);

    public void Start(int pairs)
    {
        if (!MemoryRules.IsValidSize(pairs))
        {
            throw new ArgumentOutOfRangeException(nameof(pairs), pairs, null);
        }

        var faces = MemoryFaces.All
            .OrderBy(_ => random.Next())
            .Take(pairs)
            .SelectMany(face => new[] { face, face })
            .OrderBy(_ => random.Next());

        _cards.Clear();
        _cards.AddRange(faces.Select(face => new MemoryCard(face)));

        Pairs = pairs;
        Moves = 0;
        MatchesFound = 0;
        _firstIndex = null;
        _secondIndex = null;
        IsStarted = true;
    }

    public MemoryFlipOutcome Flip(int index)
    {
        if (!IsStarted || IsWon || IsCheckingPair) return MemoryFlipOutcome.Ignored;
        if (index < 0 || index >= _cards.Count) return MemoryFlipOutcome.Ignored;

        var card = _cards[index];
        if (card.IsShown) return MemoryFlipOutcome.Ignored;

        card.IsFaceUp = true;

        if (_firstIndex is null)
        {
            _firstIndex = index;
            return MemoryFlipOutcome.FirstCard;
        }

        _secondIndex = index;
        Moves++;
        return MemoryFlipOutcome.SecondCard;
    }

    public MemoryPairOutcome ResolvePair()
    {
        if (_firstIndex is not { } firstIndex || _secondIndex is not { } secondIndex)
        {
            return MemoryPairOutcome.None;
        }

        var first = _cards[firstIndex];
        var second = _cards[secondIndex];
        _firstIndex = null;
        _secondIndex = null;

        first.IsFaceUp = false;
        second.IsFaceUp = false;

        if (first.Face != second.Face) return MemoryPairOutcome.Mismatch;

        first.IsMatched = true;
        second.IsMatched = true;
        MatchesFound++;
        return IsWon ? MemoryPairOutcome.Won : MemoryPairOutcome.Match;
    }
}
