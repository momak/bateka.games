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

    /// <summary>A Mastermind game; <paramref name="points"/> null = lost (every guess used).</summary>
    public static GameResult Mastermind(string player, int? points, int guesses, int seconds, int minute,
        int columns = 4, int colours = 6, bool repeats = true) =>
        New(player, GameKind.Mastermind, points is null ? GameOutcome.Loss : GameOutcome.Win, minute, r =>
        {
            r.Score = points;
            r.Moves = guesses;
            r.DurationMs = seconds * 1000;
            r.Columns = columns;
            r.Colours = colours;
            r.AllowRepeats = repeats;
            r.GuessLimit = 10;
        });

    private static GameResult New(string player, GameKind game, GameOutcome outcome, int minute, Action<GameResult> fill)
    {
        var result = new GameResult { Id = Guid.NewGuid(), UserId = player, Game = game, Outcome = outcome, PlayedAt = At(minute) };
        fill(result);
        return result;
    }
}
