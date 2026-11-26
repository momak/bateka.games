namespace bateka.games.Engine.TicTacToe;

/// <summary>Reading a 3 × 3 board (squares 0–8, row by row). Shared by the game and the AI.</summary>
public static class TicTacToeBoard
{
    public const int Size = 9;

    public static readonly IReadOnlyList<int[]> WinningLines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
        [0, 4, 8], [2, 4, 6]             // diagonals
    ];

    /// <summary>The line of three that wins, or null.</summary>
    public static int[]? WinningLine(IReadOnlyList<CellState> board) =>
        WinningLines.FirstOrDefault(line =>
            board[line[0]] != CellState.Empty &&
            board[line[0]] == board[line[1]] &&
            board[line[1]] == board[line[2]]);

    /// <summary>X, O, or Empty when nobody has three in a row.</summary>
    public static CellState Winner(IReadOnlyList<CellState> board) =>
        WinningLine(board) is { } line ? board[line[0]] : CellState.Empty;

    public static List<int> EmptyCells(IReadOnlyList<CellState> board) =>
        Enumerable.Range(0, Size).Where(i => board[i] == CellState.Empty).ToList();
}
