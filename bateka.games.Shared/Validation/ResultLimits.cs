using bateka.games.Engine.ConnectFour;
using bateka.games.Engine.Snake;
using bateka.games.Engine.TicTacToe;

namespace bateka.games.Shared.Validation;

/// <summary>
/// What a real game can produce. The bounds come from the engine's rules, so a cell or move
/// count the game can't reach is refused.
/// </summary>
public static class ResultLimits
{
    /// <summary>The whole Snake board.</summary>
    public const int MaxSnakeLength = SnakeRules.GridSize * SnakeRules.GridSize;

    /// <summary>Generous: every cell eaten at the highest level.</summary>
    public const int MaxSnakeScore = MaxSnakeLength * SnakeRules.PointsPerFood * 100;

    /// <summary>X, O, X, O, X is the quickest finish (a win in three of yours).</summary>
    public const int MinTicTacToeMoves = 5;
    public const int MaxTicTacToeMoves = TicTacToeBoard.Size;

    /// <summary>Your own discs: the computer can win after three of yours; at most half the board.</summary>
    public const int MinConnectFourMoves = ConnectFourBoard.InARow - 1;
    public const int MaxConnectFourMoves = (ConnectFourBoard.Rows * ConnectFourBoard.Columns + 1) / 2;

    /// <summary>A Memory game left running for a day is not a result.</summary>
    public const int MaxDurationMs = 24 * 60 * 60 * 1000;

    /// <summary>Far more pair turns than any real game.</summary>
    public const int MaxMemoryMovesPerPair = 50;
}
