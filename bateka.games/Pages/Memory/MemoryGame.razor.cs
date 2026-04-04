using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace bateka.games.Pages.Memory;

public partial class MemoryGame : ComponentBase, IDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    public static readonly int[] AvailableSizes = [3, 4, 5, 6, 7, 8];

    private static readonly List<string> EmojiPool =
    [
        "🐶","🐱","🐭","🐹","🐰","🦊","🐻","🐼",
        "🐨","🐯","🦁","🐮","🐸","🐵","🦄","🐙",
        "🦋","🐬","🦖","🦕","🐳","🦀","🐝","🦎"
    ];

    // State
    public MemoryStats Stats { get; private set; } = new();
    public List<MemoryCard> _cards { get; private set; } = [];

    private int _selectedPairs = 6;
    private int _moves = 0;
    private int _matchesFound = 0;
    private bool _gameStarted = false;
    private bool _showWinDialog = false;
    private bool _isNewBest = false;
    private bool _isChecking = false;

    // Timer
    private System.Timers.Timer? _timer;
    private double _elapsed = 0;

    // Flip tracking
    private int? _firstFlippedIndex = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadStats();
    }

    // ── Setup ────────────────────────────────────────────────

    public void StartGame()
    {
        var emojis = EmojiPool
            .OrderBy(_ => Random.Shared.Next())
            .Take(_selectedPairs)
            .SelectMany(e => new[] { e, e })
            .OrderBy(_ => Random.Shared.Next())
            .Select(e => new MemoryCard { Emoji = e })
            .ToList();

        _cards = emojis;
        _moves = 0;
        _matchesFound = 0;
        _firstFlippedIndex = null;
        _isChecking = false;
        _elapsed = 0;
        _showWinDialog = false;
        _isNewBest = false;
        _gameStarted = true;

        StartTimer();
    }

    public void RestartGame() => StartGame();

    public void BackToSetup()
    {
        StopTimer();
        _gameStarted = false;
        _showWinDialog = false;
    }

    // ── Card Click ───────────────────────────────────────────

    public async Task OnCardClick(int index)
    {
        var card = _cards[index];

        if (_isChecking || card.IsFlipped || card.IsMatched) return;
        if (_firstFlippedIndex == index) return;

        card.IsFlipped = true;

        if (_firstFlippedIndex is null)
        {
            _firstFlippedIndex = index;
            return;
        }

        // Second card flipped
        _moves++;
        _isChecking = true;
        StateHasChanged();

        await Task.Delay(800);

        var first = _cards[_firstFlippedIndex.Value];
        var second = _cards[index];

        if (first.Emoji == second.Emoji)
        {
            first.IsMatched = true;
            second.IsMatched = true;
            _matchesFound++;

            if (_matchesFound == _selectedPairs)
                await OnGameWon();
        }
        else
        {
            first.IsFlipped = false;
            second.IsFlipped = false;
        }

        _firstFlippedIndex = null;
        _isChecking = false;
    }

    // ── Win ──────────────────────────────────────────────────

    private async Task OnGameWon()
    {
        StopTimer();
        Stats.GamesPlayed++;
        _isNewBest = UpdateBestScore(_selectedPairs, _moves, _elapsed);
        await SaveStats();
        _showWinDialog = true;
    }

    private bool UpdateBestScore(int pairs, int moves, double time)
    {
        var existing = Stats.BestScores.FirstOrDefault(s => s.Pairs == pairs);
        if (existing is null)
        {
            Stats.BestScores.Add(new SizeStat { Pairs = pairs, Moves = moves, BestTime = time });
            return true;
        }

        bool better = moves < existing.Moves || (moves == existing.Moves && time < existing.BestTime);
        if (better)
        {
            existing.Moves = moves;
            existing.BestTime = time;
        }
        return better;
    }

    // ── Timer ────────────────────────────────────────────────

    private void StartTimer()
    {
        StopTimer();
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (_, _) =>
        {
            _elapsed++;
            InvokeAsync(StateHasChanged);
        };
        _timer.Start();
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
    }

    public void Dispose() => StopTimer();

    // ── Helpers ──────────────────────────────────────────────

    public SizeStat? GetBestStat(int pairs) =>
        Stats.BestScores.FirstOrDefault(s => s.Pairs == pairs);

    public static string FormatTime(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    public static string GetGridDimensions(int pairs)
    {
        int total = pairs * 2;
        return total switch
        {
            6 => "2 × 3",
            8 => "2 × 4",
            10 => "2 × 5",
            12 => "3 × 4",
            14 => "2 × 7",
            16 => "4 × 4",
            _ => $"? × ?"
        };
    }

    public string GetGridStyle()
    {
        int total = _cards.Count;
        int cols = total switch
        {
            6 => 3,
            8 => 4,
            10 => 5,
            12 => 4,
            14 => 7,
            16 => 4,
            _ => 4
        };
        return $"grid-template-columns: repeat({cols}, 1fr);";
    }

    // ── Persistence ──────────────────────────────────────────

    private async Task SaveStats()
    {
        await JS.InvokeVoidAsync("localStorage.setItem", "memory_stats",
            System.Text.Json.JsonSerializer.Serialize(Stats));
    }

    private async Task LoadStats()
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", "memory_stats");
            if (!string.IsNullOrEmpty(json))
                Stats = System.Text.Json.JsonSerializer.Deserialize<MemoryStats>(json) ?? new();
        }
        catch
        {
            Stats = new MemoryStats();
        }
    }
}