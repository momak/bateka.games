using bateka.games.Engine.Mastermind;

namespace bateka.games.Presentation;

/// <summary>
/// How the engine's colours 0…7 look. Picked to stay apart for most colour-blind players too; the
/// numbers (on by default) make them certain.
/// </summary>
public static class MastermindPalette
{
    private const string Light = "#ffffff";
    private const string Dark = "#1a1625";

    public static readonly IReadOnlyList<MastermindColour> Colours =
    [
        new("Green", "#2bb44a", Light),
        new("Purple", "#b93cc9", Light),
        new("Blue", "#2557d6", Light),
        new("Yellow", "#e0b02a", Dark),
        new("Red", "#c8102e", Light),
        new("White", "#f4f3f8", Dark),
        new("Orange", "#f57c00", Dark),
        new("Teal", "#00a5b5", Light)
    ];

    static MastermindPalette()
    {
        if (Colours.Count < MastermindRules.MaxColours)
        {
            throw new InvalidOperationException("The palette needs a colour for every colour a board can have.");
        }
    }

    /// <summary>The number shown on a peg: 1-based, like the sample.</summary>
    public static int NumberOf(int colour) => colour + 1;
}
