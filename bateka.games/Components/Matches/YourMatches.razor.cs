using bateka.games.Services;
using bateka.games.Shared.Contracts.Matches;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Components.Matches;

public partial class YourMatches : BusyComponentBase
{
    [Inject] private MatchesApiClient Matches { get; set; } = default!;
    [Inject] private CurrentPlayer Player { get; set; } = default!;

    private string? _me;
    private List<MatchDto>? _open;

    protected override async Task OnInitializedAsync()
    {
        _me = await LoadAsync(() => Player.GetUsernameAsync());
        if (_me is null) return;

        var mine = await LoadAsync(() => Matches.ListMineAsync()) ?? [];

        _open = mine
            .Where(m => m.Status is MatchStatus.Waiting or MatchStatus.Playing)
            .OrderByDescending(m => m.PlayerToMove == _me)
            .ThenByDescending(m => m.Status == MatchStatus.Playing)
            .ThenBy(m => m.CreatedAt)
            .ToList();
    }
}
