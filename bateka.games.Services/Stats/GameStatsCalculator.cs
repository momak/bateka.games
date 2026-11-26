using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Stats;

namespace bateka.games.Services.Stats;

/// <summary>
/// One user's stats from their results. Pure: no storage, no clock — so every number on the
/// stats bars is unit-tested, and the leaderboards can reuse the same rules.
/// </summary>
public static class GameStatsCalculator
{
    private const double MillisecondsPerSecond = 1000;

    public static MyStatsDto For(IEnumerable<GameResult> results)
    {
        var byGame = results.ToLookup(r => r.Game);

        return new MyStatsDto(
            Snake(byGame[GameKind.Snake]),
            Memory(byGame[GameKind.Memory]),
            TicTacToe(byGame[GameKind.TicTacToe]),
            ConnectFour(byGame[GameKind.ConnectFour]));
    }

    private static SnakeStatsDto Snake(IEnumerable<GameResult> games)
    {
        var list = games.ToList();
        if (list.Count == 0) return SnakeStatsDto.Empty;

        return new SnakeStatsDto(
            HighScore: list.Max(g => g.Score ?? 0),
            GamesPlayed: list.Count,
            LongestSnake: list.Max(g => g.Length ?? 0),
            AverageScore: (int)Math.Round(list.Average(g => g.Score ?? 0)));
    }

    private static MemoryStatsDto Memory(IEnumerable<GameResult> games)
    {
        var list = games.Where(g => g.Pairs is not null).ToList();
        if (list.Count == 0) return MemoryStatsDto.Empty;

        var bests = list
            .GroupBy(g => g.Pairs!.Value)
            .OrderBy(g => g.Key)
            .Select(g => g.OrderBy(x => x.Moves).ThenBy(x => x.DurationMs).First())
            .Select(best => new MemoryBestDto(
                best.Pairs!.Value,
                best.Moves ?? 0,
                (best.DurationMs ?? 0) / MillisecondsPerSecond))
            .ToList();

        return new MemoryStatsDto(list.Count, bests);
    }

    private static TicTacToeStatsDto TicTacToe(IEnumerable<GameResult> games)
    {
        var list = games.ToList();
        return new TicTacToeStatsDto(
            Wins: list.Count(g => g.Outcome == GameOutcome.Win),
            Losses: list.Count(g => g.Outcome == GameOutcome.Loss),
            Draws: list.Count(g => g.Outcome == GameOutcome.Draw),
            TotalMoves: list.Sum(g => g.Moves ?? 0));
    }

    private static ConnectFourStatsDto ConnectFour(IEnumerable<GameResult> games)
    {
        int played = 0, wins = 0, losses = 0, draws = 0, streak = 0, longest = 0;

        foreach (var game in games.OrderBy(g => g.PlayedAt))
        {
            played++;
            switch (game.Outcome)
            {
                case GameOutcome.Win:
                    wins++;
                    streak++;
                    longest = Math.Max(longest, streak);
                    break;
                case GameOutcome.Loss:
                    losses++;
                    streak = 0;
                    break;
                default:
                    draws++;
                    streak = 0;
                    break;
            }
        }

        return new ConnectFourStatsDto(played, wins, losses, draws, streak, longest);
    }
}
