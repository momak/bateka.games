using bateka.games.Services;
using bateka.games.Shared.Contracts.Matches;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Components.Matches;

/// <summary>Open matches on the Games page, your turn first — kept live through the hub.</summary>
public partial class YourMatches : BusyComponentBase, IAsyncDisposable
{
    [Inject] private MatchesApiClient Matches { get; set; } = default!;
    [Inject] private MatchHubClient Hub { get; set; } = default!;
    [Inject] private CurrentPlayer Player { get; set; } = default!;

    private string? _me;
    private List<MatchDto> _all = [];
    private List<MatchDto>? _open;

    protected override async Task OnInitializedAsync()
    {
        _me = await LoadAsync(() => Player.GetUsernameAsync());
        if (_me is null) return;

        Hub.MyMatchChanged += OnMyMatchChanged;
        await StartHubAsync();

        _all = await LoadAsync(() => Matches.ListMineAsync()) ?? [];
        Refresh();
    }

    /// <summary>An invite for me, a join, a move, the end: replace that match in the list.</summary>
    private void OnMyMatchChanged(MatchDto match) =>
        _ = InvokeAsync(() =>
        {
            _all = [match, .. _all.Where(m => m.Id != match.Id)];
            Refresh();
            StateHasChanged();
        });

    private void Refresh() =>
        _open = _all
            .Where(m => m.Status is MatchStatus.Waiting or MatchStatus.Playing)
            .OrderByDescending(m => m.PlayerToMove == _me || (m.Status == MatchStatus.Waiting && m.Invited == _me))
            .ThenByDescending(m => m.Status == MatchStatus.Playing)
            .ThenBy(m => m.CreatedAt)
            .ToList();

    private async Task StartHubAsync()
    {
        try
        {
            await Hub.StartAsync();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // no live updates; the list is still right as of loading the page
        }
    }

    public ValueTask DisposeAsync()
    {
        Hub.MyMatchChanged -= OnMyMatchChanged;
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
