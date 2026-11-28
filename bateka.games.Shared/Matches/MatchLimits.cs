namespace bateka.games.Shared.Matches;

/// <summary>The clock of online matches.</summary>
public static class MatchLimits
{
    public const int OneMinute = 60;
    public const int FiveMinutes = 5 * 60;
    public const int OneDay = 24 * 60 * 60;

    /// <summary>Time per move, picked when the match is created.</summary>
    public static readonly IReadOnlyList<int> MoveSecondsOptions = [OneMinute, FiveMinutes, OneDay];

    /// <summary>An invite nobody joins is cancelled after this.</summary>
    public static readonly TimeSpan InviteLifetime = TimeSpan.FromHours(24);

    /// <summary>How many finished matches "my matches" lists besides the open ones.</summary>
    public const int RecentFinished = 20;
}
