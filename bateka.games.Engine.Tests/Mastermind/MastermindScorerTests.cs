using bateka.games.Engine.Mastermind;

namespace bateka.games.Engine.Tests.Mastermind;

public sealed class MastermindScorerTests
{
    [Theory]
    [InlineData(new[] { 0, 1, 2, 3 }, new[] { 0, 1, 2, 3 }, 4, 0)] // cracked
    [InlineData(new[] { 0, 1, 2, 3 }, new[] { 3, 2, 1, 0 }, 0, 4)] // all there, all moved
    [InlineData(new[] { 0, 1, 2, 3 }, new[] { 4, 4, 5, 5 }, 0, 0)] // nothing
    [InlineData(new[] { 0, 0, 1, 1 }, new[] { 0, 1, 0, 1 }, 2, 2)]
    [InlineData(new[] { 0, 0, 0, 1 }, new[] { 0, 1, 1, 1 }, 2, 0)] // repeats aren't counted twice
    [InlineData(new[] { 1, 2, 3, 4 }, new[] { 1, 1, 2, 2 }, 1, 1)]
    [InlineData(new[] { 5, 5, 5, 5, 5 }, new[] { 5, 0, 0, 0, 0 }, 1, 0)]
    public void Scores_exact_and_misplaced_pegs(int[] code, int[] guess, int exact, int misplaced)
    {
        Assert.Equal(new MastermindFeedback(exact, misplaced), MastermindScorer.Score(code, guess));
    }

    [Fact]
    public void A_guess_of_the_wrong_length_is_refused()
    {
        Assert.Throws<ArgumentException>(() => MastermindScorer.Score([0, 1, 2, 3], [0, 1, 2]));
    }
}
