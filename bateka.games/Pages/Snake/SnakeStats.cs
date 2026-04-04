namespace bateka.games.Pages.Snake;

public class SnakeStats
{
    public int HighScore { get; set; }
    public int GamesPlayed { get; set; }
    public int LongestSnake { get; set; }
    public int TotalScore { get; set; }
    public int AverageScore => GamesPlayed == 0 ? 0
        : (int)Math.Round((double)TotalScore / GamesPlayed);
}