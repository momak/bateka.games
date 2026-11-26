namespace bateka.games.Engine.Memory;

/// <summary>One card on the Memory board. Only <see cref="MemoryGame"/> turns it.</summary>
public sealed class MemoryCard(string face)
{
    /// <summary>What's on the hidden side (an emoji).</summary>
    public string Face { get; } = face;

    public bool IsFaceUp { get; internal set; }

    public bool IsMatched { get; internal set; }

    /// <summary>Whether the page shows the face: turned over, or already part of a pair.</summary>
    public bool IsShown => IsFaceUp || IsMatched;
}
