using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Services.Boards;

/// <summary>
/// How a board ranks players. Snake: best score. Memory: fewest moves, then fastest. Tic Tac Toe and
/// Connect Four: most wins, then best win rate. A tie goes to whoever got there first.
/// </summary>
public static class BoardRules
{
    private const double MillisecondsPerSecond = 1000;
    private const int PercentDecimals = 1;

    /// <summary>The player's standing from their results on that board (any order), or null with none.</summary>
    public static BoardEntry? EntryFor(BoardKey board, string player, IEnumerable<GameResult> results)
    {
        var games = results.Where(board.Contains).OrderBy(r => r.PlayedAt).ToList();
        if (games.Count == 0) return null;

        return board.Game switch
        {
            GameKind.Snake => SnakeEntry(player, games),
            GameKind.Memory => MemoryEntry(player, games),
            _ => WinsEntry(player, games)
        };
    }

    /// <summary>Below zero: <paramref name="a"/> ranks above <paramref name="b"/>.</summary>
    public static int Compare(GameKind game, BoardEntry a, BoardEntry b)
    {
        var byValue = game == GameKind.Memory
            ? a.Value.CompareTo(b.Value)    // fewer moves first
            : b.Value.CompareTo(a.Value);   // higher score / more wins first
        if (byValue != 0) return byValue;

        var byTiebreak = game == GameKind.Memory
            ? (a.Tiebreak ?? 0).CompareTo(b.Tiebreak ?? 0)  // faster first
            : (b.Tiebreak ?? 0).CompareTo(a.Tiebreak ?? 0); // better win rate first
        if (byTiebreak != 0) return byTiebreak;

        return a.AchievedAt.CompareTo(b.AchievedAt);
    }

    public static bool IsAhead(GameKind game, BoardEntry a, BoardEntry b) => Compare(game, a, b) < 0;

    private static BoardEntry SnakeEntry(string player, List<GameResult> games)
    {
        // the first game that reached the best score
        var best = games.OrderByDescending(g => g.Score ?? 0).ThenBy(g => g.PlayedAt).First();
        return new BoardEntry(player, best.Score ?? 0, null, games.Count, best.PlayedAt);
    }

    private static BoardEntry MemoryEntry(string player, List<GameResult> games)
    {
        var best = games.OrderBy(g => g.Moves ?? int.MaxValue).ThenBy(g => g.DurationMs ?? int.MaxValue).ThenBy(g => g.PlayedAt).First();
        return new BoardEntry(player, best.Moves ?? 0, (best.DurationMs ?? 0) / MillisecondsPerSecond, games.Count, best.PlayedAt);
    }

    private static BoardEntry WinsEntry(string player, List<GameResult> games)
    {
        var wins = games.Where(g => g.Outcome == GameOutcome.Win).ToList();
        var rate = Math.Round(100.0 * wins.Count / games.Count, PercentDecimals);

        // reached this many wins with the last one (or, with none, when they started)
        var reachedAt = wins.Count > 0 ? wins[^1].PlayedAt : games[0].PlayedAt;
        return new BoardEntry(player, wins.Count, rate, games.Count, reachedAt);
    }
}
