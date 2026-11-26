namespace bateka.games.Engine.ConnectFour;

/// <summary>The computer, playing Yellow. Tries moves on the real board and takes them back.</summary>
public sealed class ConnectFourAi(Random random)
{
    /// <summary>How many moves ahead Hard looks (its own and yours).</summary>
    public const int HardDepth = 5;

    private const Disc Me = Disc.Yellow;
    private const Disc You = Disc.Red;
    private const int WinScore = 1000;
    private const int CenterDiscScore = 3;

    /// <summary>The column Yellow plays, or -1 when the board is full.</summary>
    public int ChooseColumn(ConnectFourBoard board, Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => RandomColumn(board),
        Difficulty.Medium => MediumColumn(board),
        _ => BestColumn(board, HardDepth)
    };

    private int RandomColumn(ConnectFourBoard board)
    {
        var columns = board.AvailableColumns();
        return columns.Count == 0 ? -1 : columns[random.Next(columns.Count)];
    }

    /// <summary>Win if it can, block your win if it must, otherwise random.</summary>
    private int MediumColumn(ConnectFourBoard board)
    {
        var winning = FirstWinningColumn(board, Me);
        if (winning >= 0) return winning;

        var blocking = FirstWinningColumn(board, You);
        return blocking >= 0 ? blocking : RandomColumn(board);
    }

    private static int FirstWinningColumn(ConnectFourBoard board, Disc disc)
    {
        foreach (var column in board.AvailableColumns())
        {
            board.Drop(column, disc);
            var wins = board.Winner() == disc;
            board.Undo(column);
            if (wins) return column;
        }

        return -1;
    }

    private static int BestColumn(ConnectFourBoard board, int depth)
    {
        var columns = board.AvailableColumns();
        if (columns.Count == 0) return -1;

        var bestScore = int.MinValue;
        var bestColumn = columns[0];

        foreach (var column in columns)
        {
            board.Drop(column, Me);
            var score = Minimax(board, depth - 1, isMaximizing: false, int.MinValue, int.MaxValue);
            board.Undo(column);

            if (score > bestScore)
            {
                bestScore = score;
                bestColumn = column;
            }
        }

        return bestColumn;
    }

    private static int Minimax(ConnectFourBoard board, int depth, bool isMaximizing, int alpha, int beta)
    {
        var winner = board.Winner();
        if (winner == Me) return WinScore + depth;   // win sooner
        if (winner == You) return -WinScore - depth; // lose later

        var columns = board.AvailableColumns();
        if (depth == 0 || columns.Count == 0) return ScorePosition(board);

        var best = isMaximizing ? int.MinValue : int.MaxValue;
        foreach (var column in columns)
        {
            board.Drop(column, isMaximizing ? Me : You);
            var score = Minimax(board, depth - 1, !isMaximizing, alpha, beta);
            board.Undo(column);

            if (isMaximizing)
            {
                best = Math.Max(best, score);
                alpha = Math.Max(alpha, best);
            }
            else
            {
                best = Math.Min(best, score);
                beta = Math.Min(beta, best);
            }

            if (beta <= alpha) break;
        }

        return best;
    }

    /// <summary>A quick guess at who's better off: centre discs and open twos / threes.</summary>
    private static int ScorePosition(ConnectFourBoard board)
    {
        var score = 0;

        const int center = ConnectFourBoard.Columns / 2;
        for (var row = 0; row < ConnectFourBoard.Rows; row++)
        {
            if (board[row, center] == Me) score += CenterDiscScore;
        }

        foreach (var (mine, theirs) in board.Windows(Me))
        {
            if (theirs == 0) score += WindowScore(mine);
            if (mine == 0) score -= WindowScore(theirs);
        }

        return score;
    }

    private static int WindowScore(int discs) => discs switch
    {
        4 => 100,
        3 => 5,
        2 => 2,
        _ => 0
    };
}
