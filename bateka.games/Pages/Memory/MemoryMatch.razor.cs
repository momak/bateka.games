using bateka.games.Engine.Memory;
using bateka.games.Presentation;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Stats;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Pages.Memory;

/// <summary>
/// Memory Match's page: deals and shows <see cref="MemoryGame"/>, keeps the clock, and pauses briefly
/// so you can see a pair before it's resolved. (Named MemoryMatch so it doesn't clash with the
/// engine's MemoryGame.)
/// </summary>
public partial class MemoryMatch : BusyComponentBase, IDisposable
{
    private static readonly TimeSpan ClockTick = TimeSpan.FromSeconds(1);

    [Inject] private StatsApiClient StatsApi { get; set; } = default!;
    [Inject] private ResultsApiClient ResultsApi { get; set; } = default!;

    private readonly MemoryGame _game = new(Random.Shared);

    private int _selectedPairs = MemoryRules.DefaultPairs;
    private bool _playing;
    private bool _showWinDialog;
    private bool _isNewBest;

    private int _elapsedSeconds;
    private PeriodicTimer? _clock;
    private CancellationTokenSource? _clockLoop;

    /// <summary>Null while loading (the stats bar shows a skeleton).</summary>
    private MemoryStatsDto? Stats { get; set; }

    private static IReadOnlyList<int> AvailableSizes => MemoryRules.Sizes;

    private string GridStyle => $"grid-template-columns: repeat({_game.Layout.Columns}, 1fr);";

    protected override async Task OnInitializedAsync()
    {
        var mine = await LoadAsync(() => StatsApi.GetMineAsync());
        Stats = mine?.Memory ?? MemoryStatsDto.Empty;
    }

    // ── Setup ────────────────────────────────────────────────

    private void StartGame()
    {
        _game.Start(_selectedPairs);
        _elapsedSeconds = 0;
        _showWinDialog = false;
        _isNewBest = false;
        _playing = true;
        StartClock();
    }

    private void BackToSetup()
    {
        StopClock();
        _playing = false;
        _showWinDialog = false;
    }

    // ── Cards ────────────────────────────────────────────────

    private async Task OnCardClickAsync(int index)
    {
        if (_game.Flip(index) != MemoryFlipOutcome.SecondCard) return;

        // show both cards for a moment, then match or turn them back
        StateHasChanged();
        await Task.Delay(MemoryRules.RevealMs);

        if (_game.ResolvePair() == MemoryPairOutcome.Won)
        {
            await OnGameWonAsync();
        }
    }

    private async Task OnGameWonAsync()
    {
        StopClock();
        _showWinDialog = true;

        await RunAsync(async () =>
        {
            var response = await ResultsApi.RecordAsync(ResultRequests.ForMemory(_game, _elapsedSeconds));
            Stats = response.Stats.Memory;
            _isNewBest = response.IsPersonalBest;
        });
    }

    // ── Clock ────────────────────────────────────────────────

    private void StartClock()
    {
        StopClock();
        _clockLoop = new CancellationTokenSource();
        _clock = new PeriodicTimer(ClockTick);
        _ = RunClockAsync(_clock, _clockLoop.Token);
    }

    private void StopClock()
    {
        _clockLoop?.Cancel();
        _clockLoop?.Dispose();
        _clockLoop = null;
        _clock?.Dispose();
        _clock = null;
    }

    private async Task RunClockAsync(PeriodicTimer clock, CancellationToken token)
    {
        try
        {
            while (await clock.WaitForNextTickAsync(token))
            {
                _elapsedSeconds++;
                StateHasChanged();
            }
        }
        catch (OperationCanceledException)
        {
            // won, restarted or left the page
        }
    }

    // ── Text ─────────────────────────────────────────────────

    private static string GridText(int pairs)
    {
        var layout = MemoryRules.LayoutFor(pairs);
        return $"{layout.Rows} × {layout.Columns}";
    }

    public void Dispose()
    {
        StopClock();
        GC.SuppressFinalize(this);
    }
}
