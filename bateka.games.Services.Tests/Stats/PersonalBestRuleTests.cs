using bateka.games.Services.Stats;
using bateka.games.Shared.Contracts.Games;
using R = bateka.games.Services.Tests.TestData.Results;

namespace bateka.games.Services.Tests.Stats;

public sealed class PersonalBestRuleTests
{
    [Fact]
    public void A_higher_snake_score_is_a_personal_best()
    {
        var before = GameStatsCalculator.For([R.Snake(100, 8)]);

        Assert.True(PersonalBestRule.IsPersonalBest(R.Snake(110, 9), before));
        Assert.False(PersonalBestRule.IsPersonalBest(R.Snake(100, 9), before));
    }

    [Fact]
    public void The_first_snake_game_with_points_is_a_personal_best()
    {
        var before = GameStatsCalculator.For([]);

        Assert.True(PersonalBestRule.IsPersonalBest(R.Snake(10, 4), before));
        Assert.False(PersonalBestRule.IsPersonalBest(R.Snake(0, 3), before));
    }

    [Fact]
    public void The_first_memory_game_on_a_size_is_a_personal_best()
    {
        var before = GameStatsCalculator.For([R.Memory(pairs: 6, moves: 10, seconds: 30)]);

        Assert.True(PersonalBestRule.IsPersonalBest(R.Memory(pairs: 8, moves: 30, seconds: 90), before));
    }

    [Theory]
    [InlineData(9, 60, true)]   // fewer moves, even if slower
    [InlineData(10, 29, true)]  // same moves, faster
    [InlineData(10, 30, false)] // a tie is not a new best
    [InlineData(11, 5, false)]  // more moves, however fast
    public void Memory_is_fewest_moves_then_fastest(int moves, int seconds, bool isBest)
    {
        var before = GameStatsCalculator.For([R.Memory(pairs: 6, moves: 10, seconds: 30)]);

        Assert.Equal(isBest, PersonalBestRule.IsPersonalBest(R.Memory(6, moves, seconds), before));
    }

    [Fact]
    public void Games_that_count_wins_have_no_personal_best()
    {
        var before = GameStatsCalculator.For([]);

        Assert.False(PersonalBestRule.IsPersonalBest(R.TicTacToe(GameOutcome.Win, 5), before));
        Assert.False(PersonalBestRule.IsPersonalBest(R.ConnectFour(GameOutcome.Win), before));
    }
}
