namespace bateka.games.Services.Boards;

/// <summary>
/// One player's standing on one board. <see cref="Value"/> / <see cref="Tiebreak"/> as in
/// LeaderboardRowDto. <see cref="AchievedAt"/>: when they reached it — the earlier one ranks higher
/// on a tie.
/// </summary>
public sealed record BoardEntry(string Player, int Value, double? Tiebreak, int Games, DateTime AchievedAt);
