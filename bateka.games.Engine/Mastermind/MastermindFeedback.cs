namespace bateka.games.Engine.Mastermind;

/// <summary>
/// The answer to a guess: <see cref="Exact"/> pegs are the right colour in the right place (black),
/// <see cref="Misplaced"/> are a right colour in the wrong place (white). It never says which ones.
/// </summary>
public readonly record struct MastermindFeedback(int Exact, int Misplaced)
{
    public bool Cracks(MastermindSetup setup) => Exact == setup.Pegs;
}
