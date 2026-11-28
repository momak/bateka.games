namespace bateka.games.Shared.Realtime;

/// <summary>
/// The SignalR hub for online matches: its address and the names both sides use. Under /api on
/// purpose — Bateka.BFF only sends /api/** to games-api (same as Bateka.AI's chat hub).
/// </summary>
public static class MatchHubContract
{
    /// <summary>Relative to the API's base address; the server maps it with a leading slash.</summary>
    public const string Path = "api/hubs/matches";

    /// <summary>Client → server: send me this match's updates (argument: the match id).</summary>
    public const string Watch = "Watch";

    /// <summary>Client → server: stop sending them.</summary>
    public const string Unwatch = "Unwatch";

    /// <summary>Server → client: a watched match changed (argument: the new MatchDto).</summary>
    public const string MatchUpdated = "MatchUpdated";

    /// <summary>
    /// Server → client, to each player (and the invited one) on any page: one of my matches changed —
    /// an invite for me, a join, a move, the end. Drives "Your matches" and the invite pop-up.
    /// </summary>
    public const string MyMatchChanged = "MyMatchChanged";
}
