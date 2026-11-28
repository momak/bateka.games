namespace bateka.games.Engine.Mastermind;

/// <summary>One difficulty's rules: how long the code is, how many colours, whether a colour may repeat.</summary>
public sealed record MastermindSetup(int Pegs, int Colours, bool AllowRepeats, int MaxGuesses);
