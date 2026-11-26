using bateka.games.Engine.TicTacToe;

namespace bateka.games.Stats;

public sealed class TicTacToeStats
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }

    /// <summary>Moves by both sides, over all games.</summary>
    public int TotalMoves { get; set; }

    public int Total => Wins + Losses + Draws;
    public int WinRate => Total == 0 ? 0 : (int)Math.Round((double)Wins / Total * 100);
    public double AvgMoves => Total == 0 ? 0 : Math.Round((double)TotalMoves / Total, 1);

    public void Record(TicTacToeResult result, int moves)
    {
        switch (result)
        {
            case TicTacToeResult.PlayerWin: Wins++; break;
            case TicTacToeResult.AiWin: Losses++; break;
            case TicTacToeResult.Draw: Draws++; break;
            default: return; // not finished: nothing to count
        }

        TotalMoves += moves;
    }
}
