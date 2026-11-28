namespace bateka.games.Shared.Contracts.Matches;

public sealed class MakeMoveRequest
{
    /// <summary>Tic Tac Toe: the square 0–8, row by row. Connect Four: the column 0–6.</summary>
    public int Move { get; init; }
}
