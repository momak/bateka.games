namespace bateka.games.Routing;

/// <summary>
/// The match page. Under "game/" so the Games menu link stays lit. Keep in sync with the
/// <c>@page</c> directive of MatchPage.razor.
/// </summary>
public static class MatchPageRoutes
{
    public static string For(Guid id) => $"{PageRoutes.Games}/match/{id}";
}
