using bateka.games.Domain.Entities;
using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Services.Boards;

/// <summary>
/// Which board a result counts on: Snake has one, Memory one per size, Tic Tac Toe and Connect Four
/// one per difficulty.
/// </summary>
public sealed record BoardKey(GameKind Game, Difficulty? Difficulty, int? Pairs)
{
    public static BoardKey For(GameResult result) => result.Game switch
    {
        GameKind.Memory => new(result.Game, null, result.Pairs),
        GameKind.TicTacToe or GameKind.ConnectFour => new(result.Game, result.Difficulty, null),
        _ => new(result.Game, null, null)
    };

    public bool Contains(GameResult result) => For(result) == this;
}
