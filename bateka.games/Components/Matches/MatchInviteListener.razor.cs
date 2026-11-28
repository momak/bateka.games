using bateka.games.Presentation;
using bateka.games.Routing;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Matches;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace bateka.games.Components.Matches;

/// <summary>
/// Opens the app's match connection once signed in and turns a direct invite (or rematch) for me into
/// a pop-up with an "Open" button — unless I'm already on that match's page. Each invite pops up once.
/// </summary>
public partial class MatchInviteListener : IAsyncDisposable
{
    private const string OpenAction = "Open";
    private const int VisibleMs = 15_000;

    [Inject] private MatchHubClient Hub { get; set; } = default!;
    [Inject] private CurrentPlayer Player { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ILogger<MatchInviteListener> Logger { get; set; } = default!;

    private readonly HashSet<Guid> _shown = [];
    private string? _me;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        try
        {
            _me = await Player.GetUsernameAsync();
            Hub.MyMatchChanged += OnMyMatchChanged;
            await Hub.StartAsync();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // no pop-ups this session; invites still show under "Your matches"
            Logger.LogWarning(ex, "Match invites won't pop up");
        }
    }

    private void OnMyMatchChanged(MatchDto match)
    {
        if (match.Status != MatchStatus.Waiting || match.Invited != _me || match.Creator == _me) return;
        if (!_shown.Add(match.Id)) return;

        var route = MatchPageRoutes.For(match.Id);
        if (Navigation.Uri.Contains(match.Id.ToString(), StringComparison.OrdinalIgnoreCase)) return;

        _ = InvokeAsync(() =>
            Snackbar.Add($"{match.Creator} invites you to {GameNames.Of(match.Game)}", Severity.Info, options =>
            {
                options.Action = OpenAction;
                options.ActionColor = Color.Primary;
                options.VisibleStateDuration = VisibleMs;
                options.OnClick = _ =>
                {
                    Navigation.NavigateTo(route);
                    return Task.CompletedTask;
                };
            }));
    }

    public ValueTask DisposeAsync()
    {
        Hub.MyMatchChanged -= OnMyMatchChanged;
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
