namespace bateka.games.Engine.Mastermind;

/// <summary>Scores a guess against the code, the standard way.</summary>
public static class MastermindScorer
{
    /// <summary>
    /// Exact = same colour, same place. Misplaced = for each colour, the pegs it has in both (the
    /// smaller count), minus the exact ones — so a repeated colour is never counted twice.
    /// </summary>
    public static MastermindFeedback Score(IReadOnlyList<int> code, IReadOnlyList<int> guess)
    {
        if (code.Count != guess.Count)
        {
            throw new ArgumentException("A guess must be as long as the code.", nameof(guess));
        }

        var exact = 0;
        for (var i = 0; i < code.Count; i++)
        {
            if (code[i] == guess[i]) exact++;
        }

        var inCode = code.CountBy(colour => colour).ToDictionary();
        var common = guess.CountBy(colour => colour)
            .Sum(pair => Math.Min(pair.Value, inCode.GetValueOrDefault(pair.Key)));

        return new MastermindFeedback(exact, common - exact);
    }
}
