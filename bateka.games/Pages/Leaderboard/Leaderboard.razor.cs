using bateka.games.Services;
using bateka.games.Shared.Contracts.Stats;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Pages.Leaderboard;

/// <summary>
/// Your stats from every game and the achievements they unlock. The competition increment turns
/// this into real leaderboards across Bateka users.
/// </summary>
public partial class Leaderboard : BusyComponentBase
{
    [Inject] private StatsApiClient StatsApi { get; set; } = default!;

    private bool _loaded;
    private TicTacToeStatsDto? _tttStats;
    private MemoryStatsDto? _memoryStats;
    private SnakeStatsDto? _snakeStats;
    private ConnectFourStatsDto? _cfStats;
    private List<Achievement> _achievements = [];

    private string _memoryBestLabel =>
        _memoryStats?.BestScores.OrderBy(s => s.Moves).FirstOrDefault() is { } best
            ? $"{best.Pairs} pairs, {best.Moves} moves"
            : "—";

    protected override async Task OnInitializedAsync()
    {
        var mine = await LoadAsync(() => StatsApi.GetMineAsync()) ?? MyStatsDto.Empty;
        _tttStats = mine.TicTacToe;
        _memoryStats = mine.Memory;
        _snakeStats = mine.Snake;
        _cfStats = mine.ConnectFour;
        BuildAchievements();
        _loaded = true;
    }

    private void BuildAchievements()
    {
        _achievements =
        [
            // Tic Tac Toe
            new Achievement
            {
                Icon = "🧠", Title = "First Blood",
                Description = "Win your first Tic Tac Toe game.",
                Unlocked = (_tttStats?.Wins ?? 0) >= 1
            },
            new Achievement
            {
                Icon = "🏆", Title = "Undefeated",
                Description = "Win 10 Tic Tac Toe games.",
                Unlocked = (_tttStats?.Wins ?? 0) >= 10
            },
            new Achievement
            {
                Icon = "🎯", Title = "Sharp Mind",
                Description = "Achieve a 70% win rate over 10+ games.",
                Unlocked = (_tttStats?.Total ?? 0) >= 10 && (_tttStats?.WinRate ?? 0) >= 70
            },
            new Achievement
            {
                Icon = "⚡", Title = "Speed Thinker",
                Description = "Win a game in 5 moves or fewer.",
                Unlocked = (_tttStats?.Wins ?? 0) >= 1 && (_tttStats?.AvgMoves ?? 99) <= 5
            },

            // Memory
            new Achievement
            {
                Icon = "🃏", Title = "Card Shark",
                Description = "Complete your first Memory game.",
                Unlocked = (_memoryStats?.GamesPlayed ?? 0) >= 1
            },
            new Achievement
            {
                Icon = "🧩", Title = "Perfect Memory",
                Description = "Complete an 8-pair game in under 30 moves.",
                Unlocked = _memoryStats?.BestScores
                    .Any(s => s.Pairs == 8 && s.Moves <= 30) ?? false
            },
            new Achievement
            {
                Icon = "⏱", Title = "Speed Runner",
                Description = "Complete a 6-pair game in under 30 seconds.",
                Unlocked = _memoryStats?.BestScores
                    .Any(s => s.Pairs == 6 && s.BestSeconds <= 30) ?? false
            },

            // Snake
            new Achievement
            {
                Icon = "🐍", Title = "Hatchling",
                Description = "Play your first Snake game.",
                Unlocked = (_snakeStats?.GamesPlayed ?? 0) >= 1
            },
            new Achievement
            {
                Icon = "🔥", Title = "On Fire",
                Description = "Score over 100 points in Snake.",
                Unlocked = (_snakeStats?.HighScore ?? 0) >= 100
            },
            new Achievement
            {
                Icon = "👑", Title = "Snake King",
                Description = "Score over 500 points in Snake.",
                Unlocked = (_snakeStats?.HighScore ?? 0) >= 500
            },
            new Achievement
            {
                Icon = "🦕", Title = "Massive Snake",
                Description = "Grow your snake to length 15 or more.",
                Unlocked = (_snakeStats?.LongestSnake ?? 0) >= 15
            },

            // Cross-game
            new Achievement
            {
                Icon = "🎮", Title = "Gamer",
                Description = "Play all 4 games at least once.",
                Unlocked = (_tttStats?.Total ?? 0) >= 1 &&
                           (_memoryStats?.GamesPlayed ?? 0) >= 1 &&
                           (_snakeStats?.GamesPlayed ?? 0) >= 1 &&
                           (_cfStats?.GamesPlayed ?? 0) >= 1
            },
            new Achievement
            {
                Icon = "🌟", Title = "Dedicated",
                Description = "Play a combined total of 50 games.",
                Unlocked = ((_tttStats?.Total ?? 0) +
                            (_memoryStats?.GamesPlayed ?? 0) +
                            (_snakeStats?.GamesPlayed ?? 0) +
                            (_cfStats?.GamesPlayed ?? 0)) >= 50
            },

            // Connect Four
            new Achievement
            {
                Icon = "🔴", Title = "First Drop",
                Description = "Win your first Connect Four game.",
                Unlocked = (_cfStats?.Wins ?? 0) >= 1
            },
            new Achievement
            {
                Icon = "🔥", Title = "On a Roll",
                Description = "Win 3 games in a row.",
                Unlocked = (_cfStats?.LongestWinStreak ?? 0) >= 3
            },
            new Achievement
            {
                Icon = "👑", Title = "Connect King",
                Description = "Win 10 Connect Four games.",
                Unlocked = (_cfStats?.Wins ?? 0) >= 10
            },
        ];
    }
}