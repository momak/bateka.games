using bateka.games.Shared.Contracts.Leaderboards;

namespace bateka.games.Services.Boards;

/// <summary>
/// One player's standing on one board. <see cref="Value"/> / <see cref="Tiebreak"/> as in
/// LeaderboardRowDto. <see cref="AchievedAt"/>: when they reached it — the earlier one ranks higher
/// on a tie. <see cref="Solve"/>: Mastermind's best solve (its board and guesses), else null.
/// </summary>
public sealed record BoardEntry(
    string Player, int Value, double? Tiebreak, int Games, DateTime AchievedAt, MastermindSolveDto? Solve = null);
