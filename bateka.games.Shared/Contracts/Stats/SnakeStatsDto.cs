namespace bateka.games.Shared.Contracts.Stats;

public sealed record SnakeStatsDto(int HighScore, int GamesPlayed, int LongestSnake, int AverageScore)
{
    public static readonly SnakeStatsDto Empty = new(0, 0, 0, 0);
}
