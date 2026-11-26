using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Contracts.Leaderboards;

/// <summary>One board, best first.</summary>
public sealed record LeaderboardDto(GameKind Game, Difficulty? Difficulty, int? Pairs, IReadOnlyList<LeaderboardRowDto> Rows);
