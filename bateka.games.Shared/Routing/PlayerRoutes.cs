namespace bateka.games.Shared.Routing;

public static class PlayerRoutes
{
    /// <summary>Everyone you can invite to a match (everyone who has played here, except you).</summary>
    public const string Base = "api/players";

    /// <summary>Who am I — the forwarded user (or the dev user when running without the BFF).</summary>
    public const string Me = "api/players/me";
}
