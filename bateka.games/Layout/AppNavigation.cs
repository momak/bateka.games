using bateka.games.Routing;
using Bateka.UI.Navigation;
using MudBlazor;

namespace bateka.games.Layout;

/// <summary>
/// games' pages, for the shared shell: pills in the top bar, tabs at the bottom on phones. The games
/// themselves are picked from the Games page, so the menu stays two links however many games there are.
/// </summary>
public static class AppNavigation
{
    public static readonly IReadOnlyList<BatekaNavLink> Links =
    [
        new(PageRoutes.Games, "Games", Icons.Material.Outlined.SportsEsports),
        new(PageRoutes.Leaderboard, "Leaderboard", Icons.Material.Outlined.Leaderboard)
    ];
}
