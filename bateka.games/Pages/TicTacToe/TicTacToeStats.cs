namespace bateka.games.Pages.TicTacToe;

public class TicTacToeStats
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int TotalMoves { get; set; }
    public int Total => Wins + Losses + Draws;
    public int WinRate => Total == 0 ? 0 : (int)Math.Round((double)Wins / Total * 100);
    public double AvgMoves => Total == 0 ? 0 : Math.Round((double)TotalMoves / Total, 1);
}