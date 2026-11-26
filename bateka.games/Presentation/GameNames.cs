using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Presentation;

/// <summary>How games and their boards are named on screen.</summary>
public static class GameNames
{
    public static readonly IReadOnlyList<GameKind> All =
        [GameKind.Snake, GameKind.Memory, GameKind.TicTacToe, GameKind.ConnectFour];

    public static string Of(GameKind game) => game switch
    {
        GameKind.Snake => "Snake",
        GameKind.Memory => "Memory Match",
        GameKind.TicTacToe => "Tic Tac Toe",
        _ => "Connect Four"
    };

    /// <summary>"Snake", "Memory Match · 6 pairs", "Tic Tac Toe · Hard".</summary>
    public static string Board(GameKind game, Difficulty? difficulty, int? pairs) =>
        (difficulty, pairs) switch
        {
            ({ } d, _) => $"{Of(game)} · {d}",
            (_, { } p) => $"{Of(game)} · {p} pairs",
            _ => Of(game)
        };

    public static bool HasDifficulty(GameKind game) => game is GameKind.TicTacToe or GameKind.ConnectFour;
}
