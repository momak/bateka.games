namespace bateka.games.Stats;

public sealed class MemoryStats
{
    public int GamesPlayed { get; set; }
    public List<MemoryBest> BestScores { get; set; } = [];

    public MemoryBest? Best(int pairs) => BestScores.FirstOrDefault(s => s.Pairs == pairs);

    /// <summary>Counts a won game. True when it's a new best for that board size.</summary>
    public bool Record(int pairs, int moves, double seconds)
    {
        GamesPlayed++;

        var best = Best(pairs);
        if (best is null)
        {
            BestScores.Add(new MemoryBest { Pairs = pairs, Moves = moves, BestTime = seconds });
            return true;
        }

        if (!best.IsBeatenBy(moves, seconds)) return false;

        best.Moves = moves;
        best.BestTime = seconds;
        return true;
    }
}
