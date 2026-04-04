namespace bateka.games.Pages.Snake;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

public partial class Snake : ComponentBase, IDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    // Constants
    public const int GridSize = 20;
    public const int CellSize = 24;
    public const int FoodCount = 3;
    private const int BaseInterval = 200;
    private const int MinInterval = 80;

    // Refs
    private ElementReference _canvasRef;
    private ElementReference _containerRef;

    // State
    public SnakeStats Stats { get; private set; } = new();
    private LinkedList<Point> _snake = new();
    private List<Point> _food = new();
    private Direction _direction = Direction.Right;
    private Direction _nextDirection = Direction.Right;

    private int _score = 0;
    private int _level = 1;
    private bool _gameStarted = false;
    private bool _gameOver = false;
    private bool _paused = false;
    private bool _isNewHighScore = false;
    public bool _showDPad = false;

    // Timer
    private System.Timers.Timer? _gameTimer;

    // ── Lifecycle ────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        await LoadStats();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _showDPad = await JS.InvokeAsync<bool>("eval",
                "'ontouchstart' in window || navigator.maxTouchPoints > 0");
            await JS.InvokeVoidAsync("eval",
                $"document.querySelector('.snake-container')?.focus()");
            await DrawInitialBoard();
            StateHasChanged();
        }
    }

    // ── Game Control ─────────────────────────────────────────

    public async Task StartGame()
    {
        StopTimer();

        _snake = new LinkedList<Point>();
        _snake.AddFirst(new Point(10, 10));
        _snake.AddFirst(new Point(11, 10));
        _snake.AddFirst(new Point(12, 10));

        _direction = Direction.Right;
        _nextDirection = Direction.Right;
        _score = 0;
        _level = 1;
        _gameOver = false;
        _paused = false;
        _isNewHighScore = false;
        _food = new List<Point>();

        SpawnFood();
        SpawnFood();
        SpawnFood();

        _gameStarted = true;
        StateHasChanged();

        await Draw();
        await FocusCanvas();
        StartTimer(BaseInterval);
    }

    public void TogglePause()
    {
        _paused = !_paused;
        if (_paused)
            _gameTimer?.Stop();
        else
            _gameTimer?.Start();
        StateHasChanged();
    }

    private void StartTimer(int interval)
    {
        StopTimer();
        _gameTimer = new System.Timers.Timer(interval);
        _gameTimer.Elapsed += async (_, _) => await GameTick();
        _gameTimer.AutoReset = true;
        _gameTimer.Start();
    }

    private void StopTimer()
    {
        _gameTimer?.Stop();
        _gameTimer?.Dispose();
        _gameTimer = null;
    }

    // ── Game Loop ────────────────────────────────────────────

    private async Task GameTick()
    {
        if (_paused || _gameOver) return;

        _direction = _nextDirection;
        var head = _snake.First!.Value;

        var next = _direction switch
        {
            Direction.Up => head with { Y = head.Y - 1 },
            Direction.Down => head with { Y = head.Y + 1 },
            Direction.Left => head with { X = head.X - 1 },
            Direction.Right => head with { X = head.X + 1 },
            _ => head
        };

        // Wall collision
        if (next.X < 0 || next.X >= GridSize || next.Y < 0 || next.Y >= GridSize)
        {
            await EndGame();
            return;
        }

        // Self collision (ignore tail as it will move)
        if (_snake.Skip(1).Contains(next))
        {
            await EndGame();
            return;
        }

        _snake.AddFirst(next);

        // Food collision
        var eaten = _food.FirstOrDefault(f => f.X == next.X && f.Y == next.Y);
        if (eaten is not null)
        {
            _food.Remove(eaten);
            _score += 10 * _level;
            SpawnFood();
            UpdateLevel();
        }
        else
        {
            _snake.RemoveLast();
        }

        await InvokeAsync(async () =>
        {
            await Draw();
            StateHasChanged();
        });
    }

    private void UpdateLevel()
    {
        int newLevel = 1 + (_snake.Count - 3) / 5;
        if (newLevel == _level) return;

        _level = newLevel;
        int interval = Math.Max(MinInterval, BaseInterval - (_level - 1) * 20);
        StartTimer(interval);
    }

    private async Task EndGame()
    {
        StopTimer();
        _gameOver = true;

        Stats.GamesPlayed++;
        Stats.TotalScore += _score;

        if (_score > Stats.HighScore)
        {
            Stats.HighScore = _score;
            _isNewHighScore = true;
        }

        if (_snake.Count > Stats.LongestSnake)
            Stats.LongestSnake = _snake.Count;

        await SaveStats();

        await InvokeAsync(() =>
        {
            StateHasChanged();
            return Task.CompletedTask;
        });
    }

    // ── Food ─────────────────────────────────────────────────

    private void SpawnFood()
    {
        if (_food.Count >= FoodCount) return;

        Point candidate;
        int attempts = 0;
        do
        {
            candidate = new Point(
                Random.Shared.Next(GridSize),
                Random.Shared.Next(GridSize));
            attempts++;
        }
        while ((_snake.Contains(candidate) || _food.Contains(candidate))
               && attempts < 100);

        _food.Add(candidate);
    }

    // ── Input ────────────────────────────────────────────────

    public void OnKeyDown(KeyboardEventArgs e)
    {
        var newDir = e.Key switch
        {
            "ArrowUp" or "w" or "W" => Direction.Up,
            "ArrowDown" or "s" or "S" => Direction.Down,
            "ArrowLeft" or "a" or "A" => Direction.Left,
            "ArrowRight" or "d" or "D" => Direction.Right,
            _ => _nextDirection
        };

        if (e.Key == " ")
        {
            TogglePause();
            return;
        }

        SetDirection(newDir);
    }

    public void SetDirection(Direction dir)
    {
        // Prevent reversing
        bool invalid =
            (dir == Direction.Up && _direction == Direction.Down) ||
            (dir == Direction.Down && _direction == Direction.Up) ||
            (dir == Direction.Left && _direction == Direction.Right) ||
            (dir == Direction.Right && _direction == Direction.Left);

        if (!invalid) _nextDirection = dir;
    }

    // ── Drawing ──────────────────────────────────────────────

    private async Task DrawInitialBoard()
    {
        await JS.InvokeVoidAsync("snakeDraw.drawBoard", _canvasRef, GridSize, CellSize,
            new List<object>(), new List<object>(), new List<object>(), false);
    }

    private async Task Draw()
    {
        var snakePoints = _snake.Select(p => new { p.X, p.Y }).ToList();
        var foodPoints = _food.Select(p => new { p.X, p.Y }).ToList();
        var head = _snake.First?.Value;
        var headPoint = head is not null ? new { head.X, head.Y } : null;

        await JS.InvokeVoidAsync("snakeDraw.draw", _canvasRef, GridSize, CellSize,
            snakePoints, foodPoints, headPoint, _gameOver);
    }

    private async Task FocusCanvas()
    {
        await JS.InvokeVoidAsync("eval",
            "document.querySelector('.snake-container')?.focus()");
    }

    // ── Stats ────────────────────────────────────────────────

    public async Task ResetStats()
    {
        Stats = new SnakeStats();
        await SaveStats();
        StateHasChanged();
    }

    private async Task SaveStats()
    {
        await JS.InvokeVoidAsync("localStorage.setItem", "snake_stats",
            System.Text.Json.JsonSerializer.Serialize(Stats));
    }

    private async Task LoadStats()
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", "snake_stats");
            if (!string.IsNullOrEmpty(json))
                Stats = System.Text.Json.JsonSerializer.Deserialize<SnakeStats>(json) ?? new();
        }
        catch { Stats = new SnakeStats(); }
    }

    public void Dispose() => StopTimer();
}