namespace bateka.games.Engine.Mastermind;

/// <summary>
/// One board: how long the code is (<see cref="Columns"/>), how many colours, whether a colour may
/// repeat, and how many guesses you get. The leaderboard has one board per columns × colours × repeats
/// (the guess limit doesn't change what a solve in N guesses is worth).
/// </summary>
public sealed record MastermindSetup(int Columns, int Colours, bool AllowRepeats, int Guesses);
