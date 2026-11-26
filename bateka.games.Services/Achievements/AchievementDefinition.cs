namespace bateka.games.Services.Achievements;

/// <param name="Key">Stable id — don't rename: the unlock times are worked out from it every time.</param>
public sealed record AchievementDefinition(
    string Key,
    string Icon,
    string Title,
    string Description,
    Func<AchievementContext, bool> IsUnlocked);
