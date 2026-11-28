namespace bateka.games.Shared.Matches;

/// <summary>
/// A board after some moves, the same shape for both games: <see cref="Cells"/> row by row from
/// the top-left (9 squares for Tic Tac Toe, 6 × 7 for Connect Four), null = empty.
/// </summary>
public sealed record MatchPosition(
    IReadOnlyList<MatchSide?> Cells,
    MatchSide ToMove,
    MatchSide? Winner,
    bool IsDraw,
    IReadOnlyList<int> WinningCells)
{
    public bool IsOver => Winner is not null || IsDraw;
}
