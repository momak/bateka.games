namespace bateka.games.Routing;

/// <summary>
/// Page addresses used by links and navigation. The <c>@page</c> directives must stay literal
/// (Razor needs a constant string there), so keep them in sync with these.
/// </summary>
public static class PageRoutes
{
    /// <summary>
    /// The home page. "./" (not ""): it resolves against &lt;base href&gt; like the others, and MudButton
    /// drops an empty Href — the button then renders without a link and a tap does nothing.
    /// </summary>
    public const string Home = "./";

    private const string Game = "game";

    public const string Snake = $"{Game}/snake";
    public const string Memory = $"{Game}/memory";
    public const string TicTacToe = $"{Game}/tictactoe";
    public const string ConnectFour = $"{Game}/connectfour";

    public const string Leaderboard = "leaderboard";
}
