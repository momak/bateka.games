using bateka.games.Pages.Memory;
using bateka.games.Pages.Snake;
using bateka.games.Pages.TicTacToe;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace bateka.games.Pages.Leaderboard;

public partial class Leaderboard : ComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private TicTacToeStats? _tttStats;
    private MemoryStats? _memoryStats;
    private SnakeStats? _snakeStats;
    private List<Achievement> _achievements = [];

    private string _memoryBestLabel =>
        _memoryStats?.BestScores.OrderBy(s => s.Moves).FirstOrDefault() is { } best
            ? $"{best.Pairs} pairs, {best.Moves} moves"
            : "—";

    protected override async Task OnInitializedAsync()
    {
        await LoadAllStats();
        BuildAchievements();
    }

    private async Task LoadAllStats()
    {
        _tttStats = await LoadStat<TicTacToeStats>("ttt_stats");
        _memoryStats = await LoadStat<MemoryStats>("memory_stats");
        _snakeStats = await LoadStat<SnakeStats>("snake_stats");
    }

    private async Task<T?> LoadStat<T>(string key) where T : class
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", key);
            return string.IsNullOrEmpty(json)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
        catch { return null; }
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
                    .Any(s => s.Pairs == 6 && s.BestTime <= 30) ?? false
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
                Description = "Play all 3 games at least once.",
                Unlocked = (_tttStats?.Total ?? 0) >= 1 &&
                           (_memoryStats?.GamesPlayed ?? 0) >= 1 &&
                           (_snakeStats?.GamesPlayed ?? 0) >= 1
            },
            new Achievement
            {
                Icon = "🌟", Title = "Dedicated",
                Description = "Play a combined total of 50 games.",
                Unlocked = ((_tttStats?.Total ?? 0) +
                            (_memoryStats?.GamesPlayed ?? 0) +
                            (_snakeStats?.GamesPlayed ?? 0)) >= 50
            },
        ];
    }
}