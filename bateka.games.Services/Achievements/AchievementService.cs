using bateka.games.Services.Abstractions;
using bateka.games.Shared.Contracts.Achievements;

namespace bateka.games.Services.Achievements;

/// <summary>Every achievement: when the current user unlocked it, and who else has it.</summary>
public sealed class AchievementService(IGameResultRepository results, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<AchievementDto>> GetAsync(CancellationToken ct)
    {
        var me = currentUser.UserId;
        var everyone = await results.ListAllAsync(ct);

        var unlocksByPlayer = everyone
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => AchievementTimeline.UnlockTimes(g, AchievementCatalog.All));

        var mine = unlocksByPlayer.GetValueOrDefault(me) ?? new Dictionary<string, DateTime>();

        return AchievementCatalog.All
            .Select(a => new AchievementDto(
                a.Key, a.Icon, a.Title, a.Description,
                mine.TryGetValue(a.Key, out var at) ? at : null,
                unlocksByPlayer
                    .Where(p => p.Key != me && p.Value.ContainsKey(a.Key))
                    .Select(p => new AchievementHolderDto(p.Key, p.Value[a.Key]))
                    .OrderBy(h => h.UnlockedAt)
                    .ToList()))
            .ToList();
    }
}
