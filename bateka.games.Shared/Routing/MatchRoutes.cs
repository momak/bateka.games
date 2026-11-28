namespace bateka.games.Shared.Routing;

/// <summary>Online matches. The templates are for the controller, the methods for the client.</summary>
public static class MatchRoutes
{
    public const string Base = "api/matches";
    public const string Mine = $"{Base}/mine";

    public const string ById = $"{Base}/{{id:guid}}";
    public const string Join = $"{ById}/join";
    public const string Moves = $"{ById}/moves";
    public const string Resign = $"{ById}/resign";
    public const string Rematch = $"{ById}/rematch";

    public static string For(Guid id) => $"{Base}/{id}";
    public static string JoinFor(Guid id) => $"{For(id)}/join";
    public static string MovesFor(Guid id) => $"{For(id)}/moves";
    public static string ResignFor(Guid id) => $"{For(id)}/resign";
    public static string RematchFor(Guid id) => $"{For(id)}/rematch";
}
