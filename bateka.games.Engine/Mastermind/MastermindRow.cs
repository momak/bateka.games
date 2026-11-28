namespace bateka.games.Engine.Mastermind;

/// <summary>One played row: the guess and what it scored.</summary>
public sealed record MastermindRow(IReadOnlyList<int> Guess, MastermindFeedback Feedback);
