using bateka.games.Services.Achievements;
using bateka.games.Shared.Contracts.Games;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Achievements;

public sealed class AchievementTimelineTests
{
    [Fact]
    public void An_achievement_is_dated_by_the_game_that_unlocked_it()
    {
        var unlocked = AchievementTimeline.UnlockTimes(
            [P.Snake("me", 40, 1), P.Snake("me", 120, 5), P.Snake("me", 130, 9)], AchievementCatalog.All);

        Assert.Equal(P.At(1), unlocked["snake.first"]);
        Assert.Equal(P.At(5), unlocked["snake.100"]);
        Assert.False(unlocked.ContainsKey("snake.500"));
    }

    [Fact]
    public void Gamer_needs_all_four_games()
    {
        var unlocked = AchievementTimeline.UnlockTimes(
        [
            P.Snake("me", 10, 1),
            P.Memory("me", 3, moves: 5, seconds: 20, minute: 2),
            P.TicTacToe("me", GameOutcome.Loss, 3)
        ], AchievementCatalog.All);

        Assert.False(unlocked.ContainsKey("all.gamer"));
    }

    [Fact]
    public void Every_achievement_has_a_unique_key()
    {
        var keys = AchievementCatalog.All.Select(a => a.Key).ToList();

        Assert.Equal(keys.Count, keys.Distinct().Count());
    }
}
