using bateka.games.Routing;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Components.Matches;

public partial class OnlineMatchStarter : BusyComponentBase
{
    [Inject] private MatchesApiClient Matches { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    /// <summary>Tic Tac Toe or Connect Four.</summary>
    [Parameter, EditorRequired] public GameKind Game { get; set; }

    private int _moveSeconds = MatchLimits.OneMinute;

    private async Task CreateAsync()
    {
        MatchDto? created = null;
        await RunAsync(async () =>
            created = await Matches.CreateAsync(new CreateMatchRequest { Game = Game, MoveSeconds = _moveSeconds }));

        if (created is not null)
        {
            Navigation.NavigateTo(MatchPageRoutes.For(created.Id));
        }
    }
}
