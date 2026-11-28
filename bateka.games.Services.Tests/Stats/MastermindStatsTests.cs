using bateka.games.Services.Stats;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Stats;

public sealed class MastermindStatsTests
{
    [Fact]
    public void Stats_count_games_solves_and_the_best_points()
    {
        var stats = GameStatsCalculator.For(
        [
            P.Mastermind("ana", 267, guesses: 4, seconds: 60, minute: 1),
            P.Mastermind("ana", null, guesses: 10, seconds: 300, minute: 2),
            P.Mastermind("ana", 356, guesses: 3, seconds: 45, minute: 3)
        ]).Mastermind;

        Assert.Equal(3, stats.GamesPlayed);
        Assert.Equal(2, stats.Solved);
        Assert.Equal(356, stats.BestPoints);
        Assert.Equal(3.5, stats.AverageGuesses);
        Assert.Equal(67, stats.SolveRate);
    }

    [Fact]
    public void A_solve_worth_more_than_any_before_is_a_personal_best()
    {
        var before = GameStatsCalculator.For([P.Mastermind("ana", 267, guesses: 4, seconds: 60, minute: 1)]);

        Assert.True(PersonalBestRule.IsPersonalBest(P.Mastermind("ana", 356, guesses: 3, seconds: 60, minute: 2), before));
        Assert.False(PersonalBestRule.IsPersonalBest(P.Mastermind("ana", 214, guesses: 5, seconds: 30, minute: 2), before));
        Assert.False(PersonalBestRule.IsPersonalBest(P.Mastermind("ana", null, guesses: 10, seconds: 30, minute: 2), before));
    }
}
