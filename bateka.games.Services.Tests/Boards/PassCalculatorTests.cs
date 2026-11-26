using bateka.games.Services.Boards;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Boards;

public sealed class PassCalculatorTests
{
    [Fact]
    public void Passing_someone_names_them()
    {
        var before = new[] { P.Snake("ana", 200, 1), P.Snake("marko", 300, 2), P.Snake("me", 150, 3) };

        var passed = PassCalculator.Passed(before, P.Snake("me", 250, 4));

        Assert.Equal(["ana"], passed);
    }

    [Fact]
    public void Matching_a_score_is_not_passing()
    {
        // ana got 200 first, so she stays ahead on the tie
        var before = new[] { P.Snake("ana", 200, 1) };

        Assert.Empty(PassCalculator.Passed(before, P.Snake("me", 200, 2)));
    }

    [Fact]
    public void A_first_game_on_a_board_passes_everyone_it_beats()
    {
        var before = new[] { P.Snake("ana", 100, 1), P.Snake("marko", 400, 2) };

        Assert.Equal(["ana"], PassCalculator.Passed(before, P.Snake("me", 150, 3)));
    }

    [Fact]
    public void Players_already_behind_are_not_passed_again()
    {
        var before = new[] { P.Snake("ana", 100, 1), P.Snake("me", 300, 2) };

        Assert.Empty(PassCalculator.Passed(before, P.Snake("me", 350, 3)));
    }
}
