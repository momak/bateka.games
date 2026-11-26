namespace bateka.games.Shared.Contracts.Stats;

/// <summary>The best game on one board size: fewest moves, then fastest.</summary>
public sealed record MemoryBestDto(int Pairs, int Moves, double BestSeconds);
