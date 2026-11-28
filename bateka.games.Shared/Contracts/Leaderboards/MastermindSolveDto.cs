namespace bateka.games.Shared.Contracts.Leaderboards;

/// <summary>The solve behind a Mastermind row: its board and how many guesses ("810 · 6×8 · 4 guesses").</summary>
public sealed record MastermindSolveDto(int Columns, int Colours, bool AllowRepeats, int Guesses);
