using bateka.games.Routing;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Contracts.Players;
using bateka.games.Shared.Matches;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Components.Matches;

public partial class OnlineMatchStarter : BusyComponentBase
{
    [Inject] private MatchesApiClient Matches { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private PlayersApiClient Players { get; set; } = default!;

    /// <summary>The select's "nobody in particular" value (MudSelect needs a non-null one).</summary>
    private const string AnyoneWithTheLink = "";

    /// <summary>Tic Tac Toe or Connect Four.</summary>
    [Parameter, EditorRequired] public GameKind Game { get; set; }

    private int _moveSeconds = MatchLimits.OneMinute;
    private string _opponent = AnyoneWithTheLink;

    /// <summary>Who can be invited directly; null while loading.</summary>
    private List<PlayerDto>? _players;

    protected override async Task OnInitializedAsync() =>
        _players = await LoadAsync(() => Players.ListAsync()) ?? [];

    private async Task CreateAsync()
    {
        var request = new CreateMatchRequest
        {
            Game = Game,
            MoveSeconds = _moveSeconds,
            Opponent = _opponent == AnyoneWithTheLink ? null : _opponent
        };

        MatchDto? created = null;
        await RunAsync(async () => created = await Matches.CreateAsync(request));

        if (created is not null)
        {
            Navigation.NavigateTo(MatchPageRoutes.For(created.Id));
        }
    }
}
