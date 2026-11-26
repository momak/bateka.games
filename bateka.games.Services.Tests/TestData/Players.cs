using bateka.games.Domain.Entities;
using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Services.Tests.TestData;

/// <summary>Results for named players at chosen minutes, so "who got there first" is explicit.</summary>
public static class Players
{
    public static readonly DateTime Start = new(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);

    public static DateTime At(int minute) => Start.AddMinutes(minute);

    public static GameResult Snake(string player, int score, int minute) =>
        New(player, GameKind.Snake, GameOutcome.Finished, minute, r => { r.Score = score; r.Length = 5; });

    public static GameResult Memory(string player, int pairs, int moves, int seconds, int minute) =>
        New(player, GameKind.Memory, GameOutcome.Win, minute, r => { r.Pairs = pairs; r.Moves = moves; r.DurationMs = seconds * 1000; });

    public static GameResult TicTacToe(string player, GameOutcome outcome, int minute, Difficulty difficulty = Difficulty.Hard) =>
        New(player, GameKind.TicTacToe, outcome, minute, r => { r.Moves = 7; r.Difficulty = difficulty; });

    private static GameResult New(string player, GameKind game, GameOutcome outcome, int minute, Action<GameResult> fill)
    {
        var result = new GameResult { Id = Guid.NewGuid(), UserId = player, Game = game, Outcome = outcome, PlayedAt = At(minute) };
        fill(result);
        return result;
    }
}
