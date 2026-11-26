namespace bateka.games.Stats;

public sealed class SnakeStats
{
    public int HighScore { get; set; }
    public int GamesPlayed { get; set; }
    public int LongestSnake { get; set; }
    public int TotalScore { get; set; }

    public int AverageScore => GamesPlayed == 0 ? 0 : (int)Math.Round((double)TotalScore / GamesPlayed);

    /// <summary>Counts a finished game. True when it's a new high score.</summary>
    public bool Record(int score, int length)
    {
        GamesPlayed++;
        TotalScore += score;
        LongestSnake = Math.Max(LongestSnake, length);

        if (score <= HighScore) return false;

        HighScore = score;
        return true;
    }
}
