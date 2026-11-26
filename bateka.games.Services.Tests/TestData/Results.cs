using bateka.games.Domain.Entities;
using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Services.Tests.TestData;

/// <summary>Results as the API stores them, one minute apart so the order is clear.</summary>
public static class Results
{
    private static readonly DateTime Start = new(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);
    private static int _minute;

    public static GameResult Snake(int score, int length) =>
        New(GameKind.Snake, GameOutcome.Finished, r => { r.Score = score; r.Length = length; });

    public static GameResult Memory(int pairs, int moves, int seconds) =>
        New(GameKind.Memory, GameOutcome.Win, r => { r.Pairs = pairs; r.Moves = moves; r.DurationMs = seconds * 1000; });

    public static GameResult TicTacToe(GameOutcome outcome, int moves, Difficulty difficulty = Difficulty.Hard) =>
        New(GameKind.TicTacToe, outcome, r => { r.Moves = moves; r.Difficulty = difficulty; });

    public static GameResult ConnectFour(GameOutcome outcome, Difficulty difficulty = Difficulty.Hard) =>
        New(GameKind.ConnectFour, outcome, r => { r.Moves = 10; r.Difficulty = difficulty; });

    private static GameResult New(GameKind game, GameOutcome outcome, Action<GameResult> fill)
    {
        var result = new GameResult
        {
            Id = Guid.NewGuid(),
            UserId = "ana",
            Game = game,
            Outcome = outcome,
            PlayedAt = Start.AddMinutes(Interlocked.Increment(ref _minute))
        };
        fill(result);
        return result;
    }
}
