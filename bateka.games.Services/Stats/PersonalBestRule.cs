using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Stats;

namespace bateka.games.Services.Stats;

/// <summary>
/// Whether a new result beats your own best: Snake's high score, or Memory's fewest moves (then
/// fastest) on that board size. The other games count wins, so they have no single best.
/// </summary>
public static class PersonalBestRule
{
    private const double MillisecondsPerSecond = 1000;

    /// <param name="before">Your stats without the new result.</param>
    public static bool IsPersonalBest(GameResult result, MyStatsDto before) => result.Game switch
    {
        GameKind.Snake => (result.Score ?? 0) > before.Snake.HighScore,
        GameKind.Memory => BeatsMemoryBest(result, before.Memory),
        _ => false
    };

    private static bool BeatsMemoryBest(GameResult result, MemoryStatsDto before)
    {
        if (result.Pairs is not { } pairs || result.Moves is not { } moves) return false;

        var best = before.Best(pairs);
        if (best is null) return true;

        var seconds = (result.DurationMs ?? 0) / MillisecondsPerSecond;
        return moves < best.Moves || (moves == best.Moves && seconds < best.BestSeconds);
    }
}
