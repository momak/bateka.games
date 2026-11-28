namespace bateka.games.Engine.Mastermind;

/// <summary>
/// Points for a solve: how hard the board is, per guess used. Hard = log₂ of how many codes the
/// board can hide ("bits"); points = 10 × bits² ÷ guesses. Squared so a lucky quick solve on a tiny
/// board can't beat a solid one on a big board. The guess limit doesn't count — a solve in 5 is a
/// solve in 5. Worked out by the API from the board and the guesses; the browser never sends points.
/// </summary>
public static class MastermindPoints
{
    private const double Scale = 10;

    /// <summary>Classic 4 × 6 with repeats: 1 296 codes ≈ 10.3 bits → 267 points in 4 guesses.</summary>
    public static int For(MastermindSetup setup, int guessesUsed)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(guessesUsed, 1);

        var bits = Bits(setup);
        return (int)Math.Round(Scale * bits * bits / guessesUsed, MidpointRounding.AwayFromZero);
    }

    /// <summary>log₂ of the possible codes: colours^columns with repeats, colours!/(colours−columns)! without.</summary>
    public static double Bits(MastermindSetup setup)
    {
        if (setup.AllowRepeats) return setup.Columns * Math.Log2(setup.Colours);

        var bits = 0.0;
        for (var i = 0; i < setup.Columns; i++)
        {
            bits += Math.Log2(setup.Colours - i);
        }

        return bits;
    }
}
