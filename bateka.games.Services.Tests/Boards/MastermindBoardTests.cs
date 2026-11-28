using bateka.games.Services.Boards;
using bateka.games.Services.Notes;
using bateka.games.Shared.Contracts.Games;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Boards;

public sealed class MastermindBoardTests
{
    private static readonly BoardKey Mastermind = new(GameKind.Mastermind, null, null);

    [Fact]
    public void Players_rank_by_their_best_solve_and_its_board_is_shown()
    {
        var board = LeaderboardCalculator.For(Mastermind,
        [
            P.Mastermind("ana", 267, guesses: 4, seconds: 90, minute: 1),
            P.Mastermind("ana", 810, guesses: 4, seconds: 200, minute: 2, columns: 6, colours: 8),
            P.Mastermind("marko", 535, guesses: 2, seconds: 40, minute: 3)
        ], me: "marko");

        Assert.Equal(["ana", "marko"], board.Rows.Select(r => r.Player));
        Assert.Equal(810, board.Rows[0].Value);
        Assert.Equal(2, board.Rows[0].Games);
        Assert.Equal(6, board.Rows[0].Solve!.Columns);
        Assert.Equal(8, board.Rows[0].Solve!.Colours);
        Assert.Equal(4, board.Rows[0].Solve!.Guesses);
    }

    [Fact]
    public void Only_solved_games_put_you_on_the_board()
    {
        var board = LeaderboardCalculator.For(Mastermind,
        [
            P.Mastermind("ana", null, guesses: 10, seconds: 300, minute: 1),
            P.Mastermind("marko", 107, guesses: 10, seconds: 300, minute: 2)
        ], me: "ana");

        Assert.Equal(["marko"], board.Rows.Select(r => r.Player));
    }

    [Fact]
    public void Equal_points_go_to_the_faster_solve()
    {
        var board = LeaderboardCalculator.For(Mastermind,
        [
            P.Mastermind("ana", 267, guesses: 4, seconds: 120, minute: 1),
            P.Mastermind("marko", 267, guesses: 4, seconds: 60, minute: 2)
        ], me: "ana");

        Assert.Equal(["marko", "ana"], board.Rows.Select(r => r.Player));
    }

    [Fact]
    public void A_loss_before_any_solve_passes_nobody()
    {
        var before = new[] { P.Mastermind("marko", 107, guesses: 10, seconds: 300, minute: 1) };

        Assert.Empty(PassCalculator.Passed(before, P.Mastermind("ana", null, guesses: 10, seconds: 300, minute: 2)));
    }

    [Fact]
    public void A_better_solve_passes_people()
    {
        var before = new[] { P.Mastermind("marko", 267, guesses: 4, seconds: 60, minute: 1) };

        Assert.Equal(["marko"], PassCalculator.Passed(before, P.Mastermind("ana", 356, guesses: 3, seconds: 90, minute: 2)));
    }

    [Fact]
    public void Notes_skip_a_board_you_have_only_lost_on()
    {
        var results = new[]
        {
            P.Mastermind("me", null, guesses: 10, seconds: 300, minute: 1),
            P.Mastermind("ana", 267, guesses: 4, seconds: 60, minute: 5)
        };

        Assert.Empty(NoteCalculator.For("me", results, since: P.At(0)));
    }

    [Fact]
    public void Notes_tell_you_who_passed_your_best_solve()
    {
        var results = new[]
        {
            P.Mastermind("me", 267, guesses: 4, seconds: 60, minute: 1),
            P.Mastermind("ana", null, guesses: 10, seconds: 300, minute: 3),
            P.Mastermind("ana", 535, guesses: 2, seconds: 30, minute: 6)
        };

        var note = Assert.Single(NoteCalculator.For("me", results, since: P.At(2)));
        Assert.Equal("ana", note.Player);
        Assert.Equal(535, note.TheirValue);
        Assert.Equal(P.At(6), note.PassedAt);
    }
}
