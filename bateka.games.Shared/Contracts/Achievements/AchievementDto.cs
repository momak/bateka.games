using System.Text.Json.Serialization;

namespace bateka.games.Shared.Contracts.Achievements;

/// <param name="UnlockedAt">When you unlocked it; null = not yet.</param>
/// <param name="Others">Everyone else who has it, first to unlock first.</param>
public sealed record AchievementDto(
    string Key,
    string Icon,
    string Title,
    string Description,
    DateTime? UnlockedAt,
    IReadOnlyList<AchievementHolderDto> Others)
{
    [JsonIgnore] public bool IsUnlocked => UnlockedAt is not null;
}
