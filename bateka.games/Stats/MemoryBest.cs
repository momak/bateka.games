namespace bateka.games.Stats;

/// <summary>The best game on one board size: fewest moves, then fastest.</summary>
public sealed class MemoryBest
{
    public int Pairs { get; set; }
    public int Moves { get; set; }

    /// <summary>Seconds.</summary>
    public double BestTime { get; set; }

    public bool IsBeatenBy(int moves, double seconds) =>
        moves < Moves || (moves == Moves && seconds < BestTime);
}
