using bateka.games.Domain.Entities;
using bateka.games.Services.Boards;
using bateka.games.Shared.Contracts.Notes;

namespace bateka.games.Services.Notes;

/// <summary>
/// Who went ahead of you, and when, on every board you're on. "When" is the game that first put them
/// ahead of where you stand now. Passes from before you joined a board aren't news, so they're skipped.
/// </summary>
public static class NoteCalculator
{
    /// <param name="results">Everyone's results on the boards you're on (more is fine — filtered per board).</param>
    public static IReadOnlyList<NoteDto> For(string me, IReadOnlyCollection<GameResult> results, DateTime since)
    {
        var notes = new List<NoteDto>();
        var myBoards = results.Where(r => r.UserId == me).Select(BoardKey.For).Distinct();

        foreach (var board in myBoards)
        {
            var onBoard = results.Where(board.Contains).ToList();
            var mine = onBoard.Where(r => r.UserId == me).OrderBy(r => r.PlayedAt).ToList();
            var meNow = BoardRules.EntryFor(board, me, mine)!;
            var boardSince = since > mine[0].PlayedAt ? since : mine[0].PlayedAt;

            foreach (var theirs in onBoard.Where(r => r.UserId != me).GroupBy(r => r.UserId))
            {
                if (PassedAt(board, theirs.Key, theirs, meNow) is not { } passedAt || passedAt <= boardSince) continue;

                // win rate can drop again: only a pass that still stands is news
                var themNow = BoardRules.EntryFor(board, theirs.Key, theirs)!;
                if (!BoardRules.IsAhead(board.Game, themNow, meNow)) continue;

                notes.Add(new NoteDto(board.Game, board.Difficulty, board.Pairs, theirs.Key, themNow.Value, meNow.Value, passedAt));
            }
        }

        return notes.OrderByDescending(n => n.PassedAt).Take(NoteRules.MaxNotes).ToList();
    }

    /// <summary>The game that first put <paramref name="player"/> ahead of <paramref name="meNow"/>, if any.</summary>
    private static DateTime? PassedAt(BoardKey board, string player, IEnumerable<GameResult> theirs, BoardEntry meNow)
    {
        var played = new List<GameResult>();
        foreach (var game in theirs.OrderBy(r => r.PlayedAt))
        {
            played.Add(game);
            var entry = BoardRules.EntryFor(board, player, played)!;
            if (BoardRules.IsAhead(board.Game, entry, meNow)) return game.PlayedAt;
        }

        return null;
    }
}
