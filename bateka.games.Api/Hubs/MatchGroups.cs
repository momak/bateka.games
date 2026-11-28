namespace bateka.games.Api.Hubs;

/// <summary>One SignalR group per match: every connection watching it.</summary>
public static class MatchGroups
{
    public static string For(Guid matchId) => $"match:{matchId}";
}
