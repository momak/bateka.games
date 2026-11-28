using bateka.games.Engine.ConnectFour;

namespace bateka.games.Shared.Matches;

/// <summary>Connect Four for two people: First plays Red, Second plays Yellow. Moves are columns 0–6.</summary>
internal sealed class ConnectFourMatchBoard : IMatchBoard
{
    private readonly ConnectFourBoard _board = new();

    public bool IsFull => _board.IsFull;

    public MatchMoveCheck Check(int move)
    {
        if (Winner() is not null || IsFull) return MatchMoveCheck.GameOver;
        if (move is < 0 or >= ConnectFourBoard.Columns) return MatchMoveCheck.OffTheBoard;
        return _board.CanDrop(move) ? MatchMoveCheck.Ok : MatchMoveCheck.Taken;
    }

    public void Play(int move, MatchSide side) =>
        _board.Drop(move, side == MatchSide.First ? Disc.Red : Disc.Yellow);

    public MatchSide? Winner() => ToSide(_board.Winner());

    public IReadOnlyList<MatchSide?> Cells()
    {
        var cells = new List<MatchSide?>(ConnectFourBoard.Rows * ConnectFourBoard.Columns);
        for (var row = 0; row < ConnectFourBoard.Rows; row++)
        {
            for (var column = 0; column < ConnectFourBoard.Columns; column++)
            {
                cells.Add(ToSide(_board[row, column]));
            }
        }

        return cells;
    }

    public IReadOnlyList<int> WinningCells() =>
        _board.WinningCells()
            .Select(cell => cell.Row * ConnectFourBoard.Columns + cell.Column)
            .Order()
            .ToList();

    private static MatchSide? ToSide(Disc disc) => disc switch
    {
        Disc.Red => MatchSide.First,
        Disc.Yellow => MatchSide.Second,
        _ => null
    };
}
