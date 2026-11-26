namespace bateka.games.Shared.Contracts.Leaderboards;

/// <summary>
/// One player on a board. What <see cref="Value"/> and <see cref="Tiebreak"/> mean depends on the game:
/// Snake — best score. Memory — fewest moves, then seconds. Tic Tac Toe / Connect Four — wins,
/// then win rate (%).
/// </summary>
/// <param name="Player">The Bateka username.</param>
/// <param name="Games">Games played on this board.</param>
/// <param name="IsMe">The row of whoever asked — highlighted.</param>
public sealed record LeaderboardRowDto(int Rank, string Player, int Value, double? Tiebreak, int Games, bool IsMe);
