namespace bateka.games.Engine.TicTacToe;

/// <summary>
/// One game: the player (X) always starts, the computer (O) answers. The page calls
/// <see cref="PlayerMove"/>, waits a moment so it looks like thinking, then <see cref="AiMove"/>.
/// </summary>
public sealed class TicTacToeGame(TicTacToeAi ai)
{
    private readonly CellState[] _board = new CellState[TicTacToeBoard.Size];

    public IReadOnlyList<CellState> Board => _board;

    public TicTacToeResult Result { get; private set; }

    public bool IsOver => Result != TicTacToeResult.None;

    /// <summary>The three winning squares, for highlighting; null without a winner.</summary>
    public int[]? WinningLine { get; private set; }

    /// <summary>Moves made by both sides in this game.</summary>
    public int Moves { get; private set; }

    /// <summary>The player's own moves in this game (a win "in 3 moves" counts these).</summary>
    public int PlayerMoves { get; private set; }

    public void Reset()
    {
        Array.Clear(_board);
        Result = TicTacToeResult.None;
        WinningLine = null;
        Moves = 0;
        PlayerMoves = 0;
    }

    /// <summary>Puts X on <paramref name="index"/>. False when the square is taken or the game is over.</summary>
    public bool PlayerMove(int index)
    {
        if (IsOver || index < 0 || index >= TicTacToeBoard.Size || _board[index] != CellState.Empty)
        {
            return false;
        }

        _board[index] = CellState.X;
        Moves++;
        PlayerMoves++;
        UpdateResult();
        return true;
    }

    /// <summary>The computer's answer. Returns the square it played, or -1 when there was nothing to do.</summary>
    public int AiMove(Difficulty difficulty)
    {
        if (IsOver) return -1;

        var move = ai.ChooseMove(_board, difficulty);
        if (move < 0) return -1;

        _board[move] = CellState.O;
        Moves++;
        UpdateResult();
        return move;
    }

    private void UpdateResult()
    {
        WinningLine = TicTacToeBoard.WinningLine(_board);
        Result = TicTacToeBoard.Winner(_board) switch
        {
            CellState.X => TicTacToeResult.PlayerWin,
            CellState.O => TicTacToeResult.AiWin,
            _ when TicTacToeBoard.EmptyCells(_board).Count == 0 => TicTacToeResult.Draw,
            _ => TicTacToeResult.None
        };
    }
}
