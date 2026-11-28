using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Services.Achievements;

/// <summary>Every achievement and its rule — the same ones the old Leaderboard page had, now per user on the server.</summary>
public static class AchievementCatalog
{
    private const int ManyWins = 10;
    private const int SharpMindGames = 10;
    private const int SharpMindWinRate = 70;
    private const int SpeedThinkerMoves = 5;
    private const int PerfectMemoryPairs = 8;
    private const int PerfectMemoryMoves = 30;
    private const int SpeedRunnerPairs = 6;
    private const int SpeedRunnerSeconds = 30;
    private const int OnFireScore = 100;
    private const int SnakeKingScore = 500;
    private const int MassiveSnakeLength = 15;
    private const int DedicatedGames = 50;
    private const int OnARollStreak = 3;
    private const int MindReaderGuesses = 4;
    private const int MindReaderColumns = 4;
    private const int MindReaderColours = 6;
    private const int MastermindColumns = 5;
    private const int MastermindColours = 8;

    public static readonly IReadOnlyList<AchievementDefinition> All =
    [
        // Tic Tac Toe
        new("ttt.first-win", "🧠", "First Blood", "Win your first Tic Tac Toe game.",
            c => c.Stats.TicTacToe.Wins >= 1),
        new("ttt.ten-wins", "🏆", "Undefeated", $"Win {ManyWins} Tic Tac Toe games.",
            c => c.Stats.TicTacToe.Wins >= ManyWins),
        new("ttt.sharp-mind", "🎯", "Sharp Mind", $"Reach a {SharpMindWinRate}% win rate over {SharpMindGames}+ games.",
            c => c.Stats.TicTacToe.Total >= SharpMindGames && c.Stats.TicTacToe.WinRate >= SharpMindWinRate),
        // was "average moves ≤ 5" on the old page; now what it says: one win that quick
        new("ttt.speed-thinker", "⚡", "Speed Thinker", $"Win a game in {SpeedThinkerMoves} moves or fewer.",
            c => c.Results.Any(r => r.Game == GameKind.TicTacToe && r.Outcome == GameOutcome.Win && r.Moves <= SpeedThinkerMoves)),
        new("ttt.beat-hard", "🤖", "Machine Breaker", "Beat the computer on Hard.",
            c => c.Results.Any(r => r.Game == GameKind.TicTacToe && r.Outcome == GameOutcome.Win && r.Difficulty == Difficulty.Hard)),

        // Memory
        new("memory.first", "🃏", "Card Shark", "Complete your first Memory game.",
            c => c.Stats.Memory.GamesPlayed >= 1),
        new("memory.perfect", "🧩", "Perfect Memory", $"Complete a {PerfectMemoryPairs}-pair game in {PerfectMemoryMoves} moves or fewer.",
            c => c.Stats.Memory.Best(PerfectMemoryPairs) is { } best && best.Moves <= PerfectMemoryMoves),
        new("memory.speed-runner", "⏱", "Speed Runner", $"Complete a {SpeedRunnerPairs}-pair game in {SpeedRunnerSeconds} seconds or less.",
            c => c.Results.Any(r => r.Game == GameKind.Memory && r.Pairs == SpeedRunnerPairs && r.DurationMs <= SpeedRunnerSeconds * 1000)),

        // Snake
        new("snake.first", "🐍", "Hatchling", "Play your first Snake game.",
            c => c.Stats.Snake.GamesPlayed >= 1),
        new("snake.100", "🔥", "On Fire", $"Score {OnFireScore} points or more in Snake.",
            c => c.Stats.Snake.HighScore >= OnFireScore),
        new("snake.500", "👑", "Snake King", $"Score {SnakeKingScore} points or more in Snake.",
            c => c.Stats.Snake.HighScore >= SnakeKingScore),
        new("snake.massive", "🦕", "Massive Snake", $"Grow your snake to length {MassiveSnakeLength} or more.",
            c => c.Stats.Snake.LongestSnake >= MassiveSnakeLength),

        // Connect Four
        new("cf.first-win", "🔴", "First Drop", "Win your first Connect Four game.",
            c => c.Stats.ConnectFour.Wins >= 1),
        new("cf.streak", "🔥", "On a Roll", $"Win {OnARollStreak} Connect Four games in a row.",
            c => c.Stats.ConnectFour.LongestWinStreak >= OnARollStreak),
        new("cf.ten-wins", "👑", "Connect King", $"Win {ManyWins} Connect Four games.",
            c => c.Stats.ConnectFour.Wins >= ManyWins),

        // Mastermind
        new("mm.first", "🔐", "Code Breaker", "Crack your first Mastermind code.",
            c => c.Stats.Mastermind.Solved >= 1),
        new("mm.mind-reader", "🔮", "Mind Reader",
            $"Crack a code of {MindReaderColumns}+ columns and {MindReaderColours}+ colours in {MindReaderGuesses} guesses or fewer.",
            c => c.Results.Any(r => r.Game == GameKind.Mastermind && r.Outcome == GameOutcome.Win
                                    && r.Moves <= MindReaderGuesses && r.Columns >= MindReaderColumns && r.Colours >= MindReaderColours)),
        new("mm.mastermind", "🎓", "Mastermind",
            $"Crack a code of {MastermindColumns}+ columns, {MastermindColours} colours, repeats on.",
            c => c.Results.Any(r => r.Game == GameKind.Mastermind && r.Outcome == GameOutcome.Win
                                    && r.Columns >= MastermindColumns && r.Colours >= MastermindColours && r.AllowRepeats == true)),

        // Every game
        new("all.gamer", "🎮", "Gamer", "Play all four games at least once.",
            c => c.Stats.TicTacToe.Total >= 1 && c.Stats.Memory.GamesPlayed >= 1
                 && c.Stats.Snake.GamesPlayed >= 1 && c.Stats.ConnectFour.GamesPlayed >= 1),
        new("all.dedicated", "🌟", "Dedicated", $"Play {DedicatedGames} games in total.",
            c => c.Results.Count >= DedicatedGames)
    ];
}
