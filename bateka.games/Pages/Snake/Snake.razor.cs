using bateka.games.Engine.Snake;
using bateka.games.Interop;
using bateka.games.Stats;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace bateka.games.Pages.Snake;

/// <summary>
/// Snake's page: draws <see cref="SnakeGame"/> on a canvas and runs its clock. The rules are all in
/// the engine; this only steps it, redraws and saves the stats when the snake crashes.
/// </summary>
public partial class Snake : ComponentBase, IAsyncDisposable
{
    /// <summary>Pixels per cell on the canvas (CSS scales it down on small screens).</summary>
    private const int CellSize = 24;

    [Inject] private SnakeCanvas Canvas { get; set; } = default!;
    [Inject] private LocalStatsStore StatsStore { get; set; } = default!;

    private readonly SnakeGame _game = new(Random.Shared);

    private ElementReference _canvasRef;
    private ElementReference _containerRef;

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _loop;

    private bool _paused;
    private bool _isNewHighScore;
    private bool _showDPad;

    private SnakeStats Stats { get; set; } = new();

    private static int CanvasSize => SnakeRules.GridSize * CellSize;

    private bool ShowPausedOverlay => _paused && _game.IsRunning;

    protected override async Task OnInitializedAsync()
    {
        Stats = await StatsStore.LoadAsync<SnakeStats>(StatsKeys.Snake);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        _showDPad = await Canvas.IsTouchDeviceAsync();
        await _containerRef.FocusAsync();
        await DrawAsync();
        StateHasChanged();
    }

    // ── Game control ─────────────────────────────────────────

    private async Task StartGameAsync()
    {
        _game.Start();
        _paused = false;
        _isNewHighScore = false;

        await DrawAsync();
        await _containerRef.FocusAsync();
        StartLoop();
    }

    private void TogglePause()
    {
        if (!_game.IsRunning) return;

        _paused = !_paused;
        if (_paused) StopLoop();
        else StartLoop();
    }

    private void Turn(Direction direction) => _game.Turn(direction);

    // The on-screen arrows. In C# so the markup needn't name Direction (MudBlazor has one too).
    private void TurnUp() => Turn(Direction.Up);
    private void TurnDown() => Turn(Direction.Down);
    private void TurnLeft() => Turn(Direction.Left);
    private void TurnRight() => Turn(Direction.Right);

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == SnakeKeys.Pause)
        {
            TogglePause();
            return;
        }

        if (!_paused && SnakeKeys.ToDirection(e.Key) is { } direction)
        {
            Turn(direction);
        }
    }

    // ── Clock ────────────────────────────────────────────────

    private void StartLoop()
    {
        StopLoop();
        _loop = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_game.IntervalMs));
        _ = RunLoopAsync(_timer, _loop.Token);
    }

    private void StopLoop()
    {
        _loop?.Cancel();
        _loop?.Dispose();
        _loop = null;
        _timer?.Dispose();
        _timer = null;
    }

    private async Task RunLoopAsync(PeriodicTimer timer, CancellationToken token)
    {
        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                await TickAsync(timer);
            }
        }
        catch (OperationCanceledException)
        {
            // paused, restarted or left the page
        }
    }

    private async Task TickAsync(PeriodicTimer timer)
    {
        var level = _game.Level;
        var outcome = _game.Tick();

        if (outcome == SnakeTickOutcome.Crashed)
        {
            StopLoop();
            await EndGameAsync();
        }
        else if (_game.Level != level)
        {
            // faster from the next step on
            timer.Period = TimeSpan.FromMilliseconds(_game.IntervalMs);
        }

        await DrawAsync();
        StateHasChanged();
    }

    private async Task EndGameAsync()
    {
        _isNewHighScore = Stats.Record(_game.Score, _game.Length);
        await StatsStore.SaveAsync(StatsKeys.Snake, Stats);
    }

    // ── Drawing ──────────────────────────────────────────────

    private Task DrawAsync() =>
        Canvas.DrawAsync(_canvasRef, new SnakeFrame(
            SnakeRules.GridSize, CellSize, _game.Body, _game.Food, _game.Head, _game.IsOver));

    // ── Stats ────────────────────────────────────────────────

    private async Task ResetStatsAsync()
    {
        Stats = new SnakeStats();
        await StatsStore.SaveAsync(StatsKeys.Snake, Stats);
    }

    public ValueTask DisposeAsync()
    {
        StopLoop();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
