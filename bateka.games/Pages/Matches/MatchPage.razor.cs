using bateka.games.Interop;
using bateka.games.Presentation;
using bateka.games.Routing;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace bateka.games.Pages.Matches;

/// <summary>
/// One online match: the invite while waiting, the live board while playing, the result after.
/// Every action is a REST call; the other player's moves arrive through <see cref="MatchHubClient"/>.
/// Order matters: watch first, then read, so no change slips in between — and the same again after
/// a reconnect, because pushes sent while the phone was locked are lost.
/// </summary>
public partial class MatchPage : BusyComponentBase, IAsyncDisposable
{
    private const string LinkCopied = "Link copied — paste it in a message.";
    private const string CopyFailed = "Couldn't copy — select the link and copy it.";
    private const string ShareTitle = "Bateka games";
    private static readonly TimeSpan LowTime = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan ClockTick = TimeSpan.FromSeconds(1);

    [Inject] private MatchesApiClient Matches { get; set; } = default!;
    [Inject] private MatchHubClient Hub { get; set; } = default!;
    [Inject] private CurrentPlayer Player { get; set; } = default!;
    [Inject] private ShareInterop Share { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Parameter] public Guid Id { get; set; }

    private MatchDto? _match;
    private MatchPosition? _position;
    private string? _me;
    private bool _loading = true;
    private bool _canShare;
    private bool _confirmResign;
    private bool _watching;

    private PeriodicTimer? _clock;
    private CancellationTokenSource? _clockStop;

    private string Title => _match is null ? "Online match" : $"{GameNames.Of(_match.Game)} online";

    private bool IsCreator => _match?.Creator == _me;

    private string InviteUrl => Navigation.ToAbsoluteUri(MatchPageRoutes.For(Id)).ToString();

    private bool CanPlay =>
        _match is { Status: MatchStatus.Playing } && _match.PlayerToMove == _me && !IsBusy;

    private TimeSpan? TimeLeft =>
        _match is { Status: MatchStatus.Playing, MoveDeadline: { } deadline } ? deadline - DateTime.UtcNow : null;

    private string? Countdown => TimeLeft is { } left ? MatchFormat.Remaining(left) : null;

    private bool IsLow => TimeLeft is { } left && left < LowTime;

    private Color StatusColor => _match switch
    {
        { Status: MatchStatus.Finished } m when m.Winner == _me => Color.Success,
        { Status: MatchStatus.Finished, Winner: not null } => Color.Error,
        { Status: MatchStatus.Finished } => Color.Warning,
        { Status: MatchStatus.Playing } m when m.PlayerToMove == _me => Color.Primary,
        _ => Color.Default
    };

    protected override async Task OnInitializedAsync()
    {
        Hub.MatchUpdated += OnMatchUpdated;
        Hub.Reconnected += OnReconnectedAsync;

        _me = await LoadAsync(() => Player.GetUsernameAsync());
        if (_me is not null)
        {
            await WatchAndReadAsync();
        }

        _canShare = await Share.CanShareAsync();
        _loading = false;
        StartClock();
    }

    /// <summary>A link to another match while this page is open (e.g. "New invite").</summary>
    protected override async Task OnParametersSetAsync()
    {
        if (_loading || _match is null || _match.Id == Id) return;

        await Hub.UnwatchAsync(_match.Id);
        _watching = false;
        _confirmResign = false;
        await WatchAndReadAsync();
    }

    // ── Live updates ─────────────────────────────────────────

    private async Task WatchAndReadAsync()
    {
        try
        {
            await Hub.WatchAsync(Id);
            _watching = true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // no live updates (the match may not exist — the read below says so); the page still works
            _watching = false;
        }

        Show(await LoadAsync(() => Matches.GetAsync(Id)));
    }

    private void OnMatchUpdated(MatchDto match)
    {
        if (match.Id != Id) return;

        // the hub calls from outside the renderer
        _ = InvokeAsync(() =>
        {
            Show(match);
            StateHasChanged();
        });
    }

    private Task OnReconnectedAsync() =>
        InvokeAsync(async () =>
        {
            await WatchAndReadAsync();
            StateHasChanged();
        });

    private void Show(MatchDto? match)
    {
        _match = match;
        _position = match is { Status: MatchStatus.Playing or MatchStatus.Finished }
            ? MatchRules.Replay(match.Game, match.Moves)
            : null;

        if (match?.Status != MatchStatus.Playing) _confirmResign = false;
    }

    // ── Actions ──────────────────────────────────────────────

    private async Task JoinAsync() =>
        await RunAsync(async () => Show(await Matches.JoinAsync(Id)));

    private async Task MoveAsync(int move)
    {
        if (!CanPlay) return;

        await RunAsync(async () => Show(await Matches.MoveAsync(Id, move)));
    }

    /// <summary>Playing: give up. Waiting (own invite): withdraw it.</summary>
    private async Task ResignAsync() =>
        await RunAsync(async () => Show(await Matches.ResignAsync(Id)));

    /// <summary>Same game, same time per move, a fresh link.</summary>
    private async Task NewMatchAsync()
    {
        if (_match is null) return;

        MatchDto? created = null;
        var request = new CreateMatchRequest { Game = _match.Game, MoveSeconds = _match.MoveSeconds };
        await RunAsync(async () => created = await Matches.CreateAsync(request));

        if (created is not null) Navigation.NavigateTo(MatchPageRoutes.For(created.Id));
    }

    private async Task ShareAsync()
    {
        if (_match is null) return;

        var text = $"Play {GameNames.Of(_match.Game)} with me";
        if (!await Share.ShareAsync(ShareTitle, text, InviteUrl))
        {
            await CopyAsync();
        }
    }

    private async Task CopyAsync()
    {
        var copied = await Share.CopyAsync(InviteUrl);
        Snackbar.Add(copied ? LinkCopied : CopyFailed, copied ? Severity.Success : Severity.Warning);
    }

    // ── Countdown ────────────────────────────────────────────

    /// <summary>Redraws the countdown every second. The server decides the end; this only shows it.</summary>
    private void StartClock()
    {
        _clockStop = new CancellationTokenSource();
        _clock = new PeriodicTimer(ClockTick);
        _ = RunClockAsync(_clock, _clockStop.Token);
    }

    private async Task RunClockAsync(PeriodicTimer clock, CancellationToken token)
    {
        try
        {
            while (await clock.WaitForNextTickAsync(token))
            {
                if (_match?.Status == MatchStatus.Playing)
                {
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // left the page
        }
    }

    public async ValueTask DisposeAsync()
    {
        Hub.MatchUpdated -= OnMatchUpdated;
        Hub.Reconnected -= OnReconnectedAsync;

        _clockStop?.Cancel();
        _clockStop?.Dispose();
        _clock?.Dispose();

        if (_watching)
        {
            try
            {
                await Hub.UnwatchAsync(Id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // the connection is already gone
            }
        }

        GC.SuppressFinalize(this);
    }
}
