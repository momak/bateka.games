using bateka.games.Engine.Snake;
using bateka.games.Interop;
using bateka.games.Presentation;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Stats;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace bateka.games.Pages.Snake;

/// <summary>
/// Snake's page: draws <see cref="SnakeGame"/> on a canvas and runs its clock. The rules are all in
/// the engine; this only steps it, redraws and sends the result to the API when the snake crashes.
/// </summary>
public partial class Snake : BusyComponentBase, IAsyncDisposable
{
    /// <summary>Pixels per cell on the canvas (CSS scales it down on small screens).</summary>
    private const int CellSize = 24;

    [Inject] private SnakeCanvas Canvas { get; set; } = default!;
    [Inject] private StatsApiClient StatsApi { get; set; } = default!;
    [Inject] private ResultsApiClient ResultsApi { get; set; } = default!;

    private readonly SnakeGame _game = new(Random.Shared);

    private ElementReference _canvasRef;
    private ElementReference _containerRef;

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _loop;

    private bool _paused;
    private bool _isNewHighScore;
    /// <summary>Phone or tablet: swipe steering, on-screen arrows, and no stats bar while playing.</summary>
    private bool _isTouch;

    private DotNetObjectReference<Snake>? _swipeReceiver;

    /// <summary>Null while loading (the stats bar shows a skeleton).</summary>
    private SnakeStatsDto? Stats { get; set; }

    private static int CanvasSize => SnakeRules.GridSize * CellSize;

    private bool ShowPausedOverlay => _paused && _game.IsRunning;

    /// <summary>On a phone the stats bar steps aside while playing, so board and arrows fit on screen.</summary>
    private bool ShowStatsBar => !(_isTouch && _game.IsRunning);

    protected override async Task OnInitializedAsync()
    {
        var mine = await LoadAsync(() => StatsApi.GetMineAsync());
        Stats = mine?.Snake ?? SnakeStatsDto.Empty;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        _isTouch = await Canvas.IsTouchDeviceAsync();
        if (_isTouch)
        {
            _swipeReceiver = DotNetObjectReference.Create(this);
            await Canvas.AttachSwipeAsync(_containerRef, _swipeReceiver);
        }

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

    /// <summary>Called by snake.js for a swipe on the board.</summary>
    [JSInvokable(SnakeCanvas.SwipeCallback)]
    public void OnSwipe(string swipe)
    {
        if (_paused || !_game.IsRunning) return;

        if (SnakeSwipes.ToDirection(swipe) is { } direction)
        {
            Turn(direction);
        }
    }

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

    private Task EndGameAsync() =>
        RunAsync(async () =>
        {
            var response = await ResultsApi.RecordAsync(ResultRequests.ForSnake(_game));
            Stats = response.Stats.Snake;
            _isNewHighScore = response.IsPersonalBest;
            if (PassedMessages.For(response.Passed) is { } message) Snackbar.Add(message, MudBlazor.Severity.Success);
        });

    // ── Drawing ──────────────────────────────────────────────

    private Task DrawAsync() =>
        Canvas.DrawAsync(_canvasRef, new SnakeFrame(
            SnakeRules.GridSize, CellSize, _game.Body, _game.Food, _game.Head, _game.IsOver));

    public async ValueTask DisposeAsync()
    {
        StopLoop();

        if (_swipeReceiver is not null)
        {
            await Canvas.DetachSwipeAsync(_containerRef);
            _swipeReceiver.Dispose();
            _swipeReceiver = null;
        }

        GC.SuppressFinalize(this);
    }
}
