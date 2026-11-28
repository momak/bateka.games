using bateka.games.Engine;
using bateka.games.Engine.ConnectFour;
using bateka.games.Engine.Mastermind;
using bateka.games.Engine.Memory;
using bateka.games.Engine.Snake;
using bateka.games.Engine.TicTacToe;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Results;

namespace bateka.games.Presentation;

/// <summary>What each finished game sends to the API — only that game's fields (the API's validator checks).</summary>
public static class ResultRequests
{
    private const int MillisecondsPerSecond = 1000;

    public static RecordGameResultRequest ForSnake(SnakeGame game) => new()
    {
        Game = GameKind.Snake,
        Outcome = GameOutcome.Finished,
        Score = game.Score,
        Length = game.Length
    };

    public static RecordGameResultRequest ForMemory(MemoryGame game, int elapsedSeconds) => new()
    {
        Game = GameKind.Memory,
        Outcome = GameOutcome.Win,
        Pairs = game.Pairs,
        Moves = game.Moves,
        DurationMs = elapsedSeconds * MillisecondsPerSecond
    };

    public static RecordGameResultRequest ForTicTacToe(TicTacToeGame game, Difficulty difficulty) => new()
    {
        Game = GameKind.TicTacToe,
        Outcome = game.Result switch
        {
            TicTacToeResult.PlayerWin => GameOutcome.Win,
            TicTacToeResult.AiWin => GameOutcome.Loss,
            _ => GameOutcome.Draw
        },
        Difficulty = difficulty,
        Moves = game.Moves
    };

    /// <summary>A solved or lost game, with its board. The points are worked out by the API.</summary>
    public static RecordGameResultRequest ForMastermind(MastermindGame game, int elapsedSeconds) => new()
    {
        Game = GameKind.Mastermind,
        Outcome = game.IsSolved ? GameOutcome.Win : GameOutcome.Loss,
        Moves = game.GuessesUsed,
        DurationMs = elapsedSeconds * MillisecondsPerSecond,
        Columns = game.Setup.Columns,
        Colours = game.Setup.Colours,
        AllowRepeats = game.Setup.AllowRepeats,
        GuessLimit = game.Setup.Guesses
    };

    /// <summary>Against the computer only; two-player games aren't sent.</summary>
    public static RecordGameResultRequest ForConnectFour(ConnectFourGame game) => new()
    {
        Game = GameKind.ConnectFour,
        Outcome = game.Result switch
        {
            ConnectFourResult.RedWins => GameOutcome.Win,
            ConnectFourResult.YellowWins => GameOutcome.Loss,
            _ => GameOutcome.Draw
        },
        Difficulty = game.Difficulty,
        Moves = game.RedMoves
    };
}
