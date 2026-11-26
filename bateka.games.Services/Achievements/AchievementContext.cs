using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Stats;

namespace bateka.games.Services.Achievements;

/// <summary>What an achievement rule looks at: a player's results so far and the stats they make.</summary>
public sealed record AchievementContext(IReadOnlyList<GameResult> Results, MyStatsDto Stats);
