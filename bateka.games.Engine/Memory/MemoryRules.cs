namespace bateka.games.Engine.Memory;

/// <summary>Board sizes and their layouts.</summary>
public static class MemoryRules
{
    public const int MinPairs = 3;
    public const int MaxPairs = 8;
    public const int DefaultPairs = 6;

    /// <summary>How long the page shows two turned-over cards before <see cref="MemoryGame.ResolvePair"/>.</summary>
    public const int RevealMs = 800;

    public static readonly IReadOnlyList<int> Sizes =
        Enumerable.Range(MinPairs, MaxPairs - MinPairs + 1).ToArray();

    public static bool IsValidSize(int pairs) => pairs is >= MinPairs and <= MaxPairs;

    /// <summary>Rows × columns for a board of <paramref name="pairs"/> pairs (the most square fit).</summary>
    public static MemoryLayout LayoutFor(int pairs) => pairs switch
    {
        3 => new(2, 3),
        4 => new(2, 4),
        5 => new(2, 5),
        6 => new(3, 4),
        7 => new(2, 7),
        8 => new(4, 4),
        _ => throw new ArgumentOutOfRangeException(nameof(pairs), pairs, null)
    };
}
