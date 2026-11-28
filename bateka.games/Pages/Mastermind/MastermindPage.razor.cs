using bateka.games.Engine.Mastermind;
using bateka.games.Interop;
using bateka.games.Presentation;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Stats;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace bateka.games.Pages.Mastermind;

/// <summary>
/// Mastermind's page: the board, the colour bar and the options panel. The rules are in
/// <see cref="MastermindGame"/>; this fills the current row, checks it, keeps the clock and sends the
/// result (with its board) when the game ends. Named MastermindPage so it doesn't clash with the
/// engine's namespace.
/// </summary>
public partial class MastermindPage : BusyComponentBase, IDisposable
{
    private const string Exact = "exact";
    private const string Misplaced = "misplaced";
    private const string None = "none";
    private const string EmptyName = "Empty";

    private static readonly TimeSpan ClockTick = TimeSpan.FromSeconds(1);

    [Inject] private StatsApiClient StatsApi { get; set; } = default!;
    [Inject] private ResultsApiClient ResultsApi { get; set; } = default!;
    [Inject] private MastermindOptionsStore OptionsStore { get; set; } = default!;

    private readonly MastermindGame _game = new(Random.Shared);
    private readonly DialogOptions _dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall };

    private MastermindOptions _options = new();
    private MastermindOptions _draft = new();
    private bool _showOptions;

    /// <summary>
    /// The row being filled; null = empty hole. Sized for the classic board from the start: the page
    /// renders once before the saved options are loaded and the game starts.
    /// </summary>
    private int?[] _current = new int?[MastermindRules.Classic.Columns];

    private bool _isNewBest;
    private int _elapsedSeconds;
    private PeriodicTimer? _clock;
    private CancellationTokenSource? _clockLoop;

    /// <summary>Null while loading (the stats bar shows a skeleton).</summary>
    private MastermindStatsDto? Stats { get; set; }

    private int CurrentRow => _game.IsOver ? -1 : _game.Rows.Count;

    private bool CanCheck =>
        _game.IsStarted && !_game.IsOver && !IsBusy && _current.All(c => c is not null)
        && MastermindRules.IsValid(_game.Setup, _current.Select(c => c!.Value).ToArray());

    private string BoardText =>
        $"{_game.Setup.Columns}×{_game.Setup.Colours}{(_game.Setup.AllowRepeats ? string.Empty : " · no repeats")}";

    /// <summary>Peg size follows the screen: the widest board (6 columns + its key dots) fits a phone.</summary>
    private string BoardStyle => $"--mm-columns: {_game.Setup.Columns};";

    private string KeysStyle => $"grid-template-columns: repeat({(_game.Setup.Columns + 1) / 2}, 1fr);";

    private int SolvePoints => MastermindPoints.For(_game.Setup, _game.GuessesUsed);

    protected override async Task OnInitializedAsync()
    {
        _options = await OptionsStore.LoadAsync();
        StartGame();

        var mine = await LoadAsync(() => StatsApi.GetMineAsync());
        Stats = mine?.Mastermind ?? MastermindStatsDto.Empty;
    }

    // ── Game ─────────────────────────────────────────────────

    private void StartGame()
    {
        _game.Start(_options.ToSetup());
        _current = new int?[_game.Setup.Columns];
        _isNewBest = false;
        _elapsedSeconds = 0;
        StartClock();
    }

    private Task NewGameAsync()
    {
        StartGame();
        return Task.CompletedTask;
    }

    private bool CanPick(int colour) =>
        _game.IsStarted && !_game.IsOver && _current.Any(c => c is null)
        && (_game.Setup.AllowRepeats || !_current.Contains(colour));

    /// <summary>A colour goes into the first empty hole of the current row.</summary>
    private void Pick(int colour)
    {
        if (!CanPick(colour)) return;

        _current[Array.IndexOf(_current, null)] = colour;
    }

    /// <summary>Tapping a peg in the current row takes it out again.</summary>
    private void ClearPeg(int column)
    {
        if (_game.IsStarted && !_game.IsOver && column < _current.Length) _current[column] = null;
    }

    private async Task CheckAsync()
    {
        if (!CanCheck) return;

        var outcome = _game.Guess(_current.Select(c => c!.Value).ToArray());
        if (outcome == MastermindGuessOutcome.Invalid) return;

        _current = new int?[_game.Setup.Columns];

        if (outcome is MastermindGuessOutcome.Solved or MastermindGuessOutcome.OutOfGuesses)
        {
            await EndAsync();
        }
    }

    private async Task EndAsync()
    {
        StopClock();

        await RunAsync(async () =>
        {
            var response = await ResultsApi.RecordAsync(ResultRequests.ForMastermind(_game, _elapsedSeconds));
            Stats = response.Stats.Mastermind;
            _isNewBest = response.IsPersonalBest;
            if (PassedMessages.For(response.Passed) is { } message) Snackbar.Add(message, Severity.Success);
        });
    }

    // ── Options ──────────────────────────────────────────────

    private void OpenOptions()
    {
        _draft = _options.Copy();
        _showOptions = true;
    }

    /// <summary>New options mean a new game (a board can't change halfway).</summary>
    private async Task ApplyOptionsAsync()
    {
        if (!_draft.IsPlayable) return;

        _options = _draft;
        _showOptions = false;
        StartGame();
        await OptionsStore.SaveAsync(_options);
    }

    // ── Board ────────────────────────────────────────────────

    private int? PegAt(int row, int column)
    {
        if (row < _game.Rows.Count) return _game.Rows[row].Guess[column];
        return row == CurrentRow && column < _current.Length ? _current[column] : null;
    }

    /// <summary>The row's key dots: black for each exact peg, then white for each misplaced one, then empty.</summary>
    private IEnumerable<string> KeysFor(int row)
    {
        var columns = _game.Setup.Columns;
        if (row >= _game.Rows.Count) return Enumerable.Repeat(None, columns);

        var feedback = _game.Rows[row].Feedback;
        return Enumerable.Repeat(Exact, feedback.Exact)
            .Concat(Enumerable.Repeat(Misplaced, feedback.Misplaced))
            .Concat(Enumerable.Repeat(None, columns - feedback.Exact - feedback.Misplaced));
    }

    private static string? PegStyle(int? colour) =>
        colour is { } c ? $"background:{MastermindPalette.Colours[c].Fill}; color:{MastermindPalette.Colours[c].Ink};" : null;

    private string PegLabel(int? colour) =>
        colour is { } c && _options.ShowNumbers ? MastermindPalette.NumberOf(c).ToString() : string.Empty;

    private static string PegName(int? colour) => colour is { } c ? MastermindPalette.Colours[c].Name : EmptyName;

    private static string GuessesText(int guesses) => guesses == 1 ? "1 guess" : $"{guesses} guesses";

    /// <summary>"A solve here is worth 267 pts in 4 guesses, 535 in 2."</summary>
    private static string WorthText(MastermindOptions options)
    {
        var setup = options.ToSetup();
        return $"A solve here is worth {MastermindPoints.For(setup, 4)} pts in 4 guesses, {MastermindPoints.For(setup, 2)} in 2.";
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
            // over, restarted or left the page
        }
    }

    public void Dispose()
    {
        StopClock();
        GC.SuppressFinalize(this);
    }
}
