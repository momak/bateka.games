using bateka.games.Domain.Entities;
using bateka.games.Services.Stats;

namespace bateka.games.Services.Achievements;

/// <summary>
/// When a player unlocked each achievement: replays their results in order and notes the first game
/// after which a rule holds. Nothing is stored, so a new achievement shows up for everyone, dated
/// correctly, the moment it's added to the catalog.
/// </summary>
public static class AchievementTimeline
{
    /// <returns>Achievement key → when it was unlocked (only the unlocked ones).</returns>
    public static IReadOnlyDictionary<string, DateTime> UnlockTimes(
        IEnumerable<GameResult> results, IReadOnlyList<AchievementDefinition> catalog)
    {
        var unlocked = new Dictionary<string, DateTime>();
        var played = new List<GameResult>();

        foreach (var result in results.OrderBy(r => r.PlayedAt))
        {
            played.Add(result);
            var context = new AchievementContext(played, GameStatsCalculator.For(played));

            foreach (var achievement in catalog)
            {
                if (!unlocked.ContainsKey(achievement.Key) && achievement.IsUnlocked(context))
                {
                    unlocked[achievement.Key] = result.PlayedAt;
                }
            }

            if (unlocked.Count == catalog.Count) break;
        }

        return unlocked;
    }
}
