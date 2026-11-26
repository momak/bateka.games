using bateka.games.Domain.Entities;

namespace bateka.games.Services.Boards;

/// <summary>Who a new result put you ahead of, on its own board ("You passed Ana!").</summary>
public static class PassCalculator
{
    /// <param name="boardBefore">Everyone's results on that board, without the new one.</param>
    public static IReadOnlyList<string> Passed(IReadOnlyCollection<GameResult> boardBefore, GameResult newResult)
    {
        var board = BoardKey.For(newResult);
        var me = newResult.UserId;

        var mine = boardBefore.Where(r => r.UserId == me).ToList();
        var meBefore = BoardRules.EntryFor(board, me, mine);
        var meAfter = BoardRules.EntryFor(board, me, [.. mine, newResult])!;

        return boardBefore
            .Where(r => r.UserId != me)
            .GroupBy(r => r.UserId)
            .Select(g => BoardRules.EntryFor(board, g.Key, g))
            .OfType<BoardEntry>()
            .Where(other =>
                // ahead of me before (or I wasn't on the board yet) …
                (meBefore is null || BoardRules.IsAhead(board.Game, other, meBefore))
                // … and behind me now
                && BoardRules.IsAhead(board.Game, meAfter, other))
            .Select(other => other.Player)
            .ToList();
    }
}
