using bateka.games.Routing;
using Bateka.UI.Navigation;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace bateka.games.Layout;

/// <summary>games' pages, for the shared shell: pills in the top bar, tabs at the bottom on phones.</summary>
public static class AppNavigation
{
    private const string MemoryTabText = "Memory";
    private const string TicTacToeTabText = "XO";
    private const string ConnectFourTabText = "Four";
    private const string LeaderboardTabText = "Board";

    public static readonly IReadOnlyList<BatekaNavLink> Links =
    [
        new(PageRoutes.Home, "Home", Icons.Material.Outlined.Home, NavLinkMatch.All),
        new(PageRoutes.Snake, "Snake", Icons.Material.Outlined.Gamepad),
        new(PageRoutes.Memory, "Memory Match", Icons.Material.Outlined.GridView, TabText: MemoryTabText),
        new(PageRoutes.TicTacToe, "Tic Tac Toe", Icons.Material.Outlined.Close, TabText: TicTacToeTabText),
        new(PageRoutes.ConnectFour, "Connect Four", Icons.Material.Outlined.GridOn, TabText: ConnectFourTabText),
        new(PageRoutes.Leaderboard, "Leaderboard", Icons.Material.Outlined.Leaderboard, TabText: LeaderboardTabText)
    ];
}
