namespace bateka.games.Shared.Contracts.Stats;

/// <param name="BestScores">One per board size played, smallest first.</param>
public sealed record MemoryStatsDto(int GamesPlayed, IReadOnlyList<MemoryBestDto> BestScores)
{
    public static readonly MemoryStatsDto Empty = new(0, []);

    public MemoryBestDto? Best(int pairs) => BestScores.FirstOrDefault(b => b.Pairs == pairs);
}
