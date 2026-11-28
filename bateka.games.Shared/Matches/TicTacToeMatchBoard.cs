using bateka.games.Engine.TicTacToe;

namespace bateka.games.Shared.Matches;

/// <summary>Tic Tac Toe for two people: First plays X, Second plays O. Moves are squares 0–8.</summary>
internal sealed class TicTacToeMatchBoard : IMatchBoard
{
    private readonly CellState[] _board = new CellState[TicTacToeBoard.Size];

    public bool IsFull => TicTacToeBoard.EmptyCells(_board).Count == 0;

    public MatchMoveCheck Check(int move)
    {
        if (Winner() is not null || IsFull) return MatchMoveCheck.GameOver;
        if (move is < 0 or >= TicTacToeBoard.Size) return MatchMoveCheck.OffTheBoard;
        return _board[move] == CellState.Empty ? MatchMoveCheck.Ok : MatchMoveCheck.Taken;
    }

    public void Play(int move, MatchSide side) =>
        _board[move] = side == MatchSide.First ? CellState.X : CellState.O;

    public MatchSide? Winner() => ToSide(TicTacToeBoard.Winner(_board));

    public IReadOnlyList<MatchSide?> Cells() => _board.Select(ToSide).ToList();

    public IReadOnlyList<int> WinningCells() => TicTacToeBoard.WinningLine(_board) ?? [];

    private static MatchSide? ToSide(CellState cell) => cell switch
    {
        CellState.X => MatchSide.First,
        CellState.O => MatchSide.Second,
        _ => null
    };
}
