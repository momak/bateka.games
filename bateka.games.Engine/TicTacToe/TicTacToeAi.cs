namespace bateka.games.Engine.TicTacToe;

/// <summary>The computer (O). Hard never loses: full minimax with alpha-beta pruning.</summary>
public sealed class TicTacToeAi(Random random)
{
    private const int WinScore = 10;

    /// <summary>The square O plays, or -1 when the board is full.</summary>
    public int ChooseMove(IReadOnlyList<CellState> board, Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => RandomMove(board),
        // half the time the best move, otherwise a random one
        Difficulty.Medium => random.Next(2) == 0 ? BestMove(board) : RandomMove(board),
        _ => BestMove(board)
    };

    private int RandomMove(IReadOnlyList<CellState> board)
    {
        var empty = TicTacToeBoard.EmptyCells(board);
        return empty.Count == 0 ? -1 : empty[random.Next(empty.Count)];
    }

    private static int BestMove(IReadOnlyList<CellState> board)
    {
        var work = board.ToArray();
        var bestScore = int.MinValue;
        var bestMove = -1;

        foreach (var i in TicTacToeBoard.EmptyCells(work))
        {
            work[i] = CellState.O;
            var score = Minimax(work, 0, isMaximizing: false, int.MinValue, int.MaxValue);
            work[i] = CellState.Empty;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = i;
            }
        }

        return bestMove;
    }

    private static int Minimax(CellState[] board, int depth, bool isMaximizing, int alpha, int beta)
    {
        var winner = TicTacToeBoard.Winner(board);
        if (winner == CellState.O) return WinScore - depth; // win sooner
        if (winner == CellState.X) return depth - WinScore; // lose later

        var empty = TicTacToeBoard.EmptyCells(board);
        if (empty.Count == 0) return 0;

        var best = isMaximizing ? int.MinValue : int.MaxValue;
        foreach (var i in empty)
        {
            board[i] = isMaximizing ? CellState.O : CellState.X;
            var score = Minimax(board, depth + 1, !isMaximizing, alpha, beta);
            board[i] = CellState.Empty;

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
}
