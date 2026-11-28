using bateka.games.Engine.Mastermind;

namespace bateka.games.Engine.Tests.Mastermind;

/// <summary>The numbers agreed with Bojan (2026-11-28) — change them only on purpose.</summary>
public sealed class MastermindPointsTests
{
    [Theory]
    [InlineData(3, 4, false, 2, 105)]
    [InlineData(4, 6, false, 4, 180)]
    [InlineData(4, 6, true, 2, 535)]
    [InlineData(4, 6, true, 4, 267)]   // classic, a good solve
    [InlineData(4, 6, true, 10, 107)]
    [InlineData(5, 8, true, 5, 450)]
    [InlineData(6, 8, true, 4, 810)]
    [InlineData(6, 8, true, 10, 324)]
    public void Points_follow_the_agreed_table(int columns, int colours, bool repeats, int guesses, int points)
    {
        var setup = new MastermindSetup(columns, colours, repeats, Guesses: 12);

        Assert.Equal(points, MastermindPoints.For(setup, guesses));
    }

    [Fact]
    public void Fewer_guesses_and_bigger_boards_are_worth_more()
    {
        Assert.True(MastermindPoints.For(MastermindRules.Classic, 3) > MastermindPoints.For(MastermindRules.Classic, 4));
        Assert.True(MastermindPoints.For(MastermindRules.Classic with { Columns = 5 }, 4) > MastermindPoints.For(MastermindRules.Classic, 4));
        Assert.True(MastermindPoints.For(MastermindRules.Classic, 4) > MastermindPoints.For(MastermindRules.Classic with { AllowRepeats = false }, 4));
    }

    [Fact]
    public void The_guess_limit_does_not_change_the_points()
    {
        Assert.Equal(
            MastermindPoints.For(MastermindRules.Classic with { Guesses = 6 }, 5),
            MastermindPoints.For(MastermindRules.Classic with { Guesses = 12 }, 5));
    }

    [Fact]
    public void Bits_count_the_possible_codes()
    {
        Assert.Equal(Math.Log2(1296), MastermindPoints.Bits(MastermindRules.Classic), precision: 9);
        Assert.Equal(Math.Log2(360), MastermindPoints.Bits(MastermindRules.Classic with { AllowRepeats = false }), precision: 9);
    }
}
