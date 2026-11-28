using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Routing;

/// <summary>
/// One game's all-time board. Snake and Mastermind have one board; Memory one per size
/// (<c>?pairs=6</c>); Tic Tac Toe and Connect Four one per difficulty (<c>?difficulty=hard</c>).
/// </summary>
public static class LeaderboardRoutes
{
    public const string Base = "api/leaderboards";
    public const string GameTemplate = $"{Base}/{{game}}";

    public const string DifficultyParameter = "difficulty";
    public const string PairsParameter = "pairs";

    public static string For(GameKind game, Difficulty? difficulty = null, int? pairs = null)
    {
        var path = $"{Base}/{game}";
        if (difficulty is { } d) return $"{path}?{DifficultyParameter}={d}";
        if (pairs is { } p) return $"{path}?{PairsParameter}={p}";
        return path;
    }
}
