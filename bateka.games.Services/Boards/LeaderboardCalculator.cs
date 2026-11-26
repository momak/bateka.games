using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Leaderboards;

namespace bateka.games.Services.Boards;

/// <summary>A board from everyone's results. Pure, like the stats calculator.</summary>
public static class LeaderboardCalculator
{
    public static IReadOnlyList<BoardEntry> Standings(BoardKey board, IEnumerable<GameResult> results) =>
        results
            .Where(board.Contains)
            .GroupBy(r => r.UserId)
            .Select(g => BoardRules.EntryFor(board, g.Key, g))
            .OfType<BoardEntry>()
            .Order(Comparer<BoardEntry>.Create((a, b) => BoardRules.Compare(board.Game, a, b)))
            .ToList();

    public static LeaderboardDto For(BoardKey board, IEnumerable<GameResult> results, string me)
    {
        var rows = Standings(board, results)
            .Select((entry, index) => new LeaderboardRowDto(
                index + 1, entry.Player, entry.Value, entry.Tiebreak, entry.Games, entry.Player == me))
            .ToList();

        return new LeaderboardDto(board.Game, board.Difficulty, board.Pairs, rows);
    }
}
