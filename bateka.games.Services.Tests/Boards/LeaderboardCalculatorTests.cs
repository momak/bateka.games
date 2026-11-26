using bateka.games.Engine;
using bateka.games.Services.Boards;
using bateka.games.Shared.Contracts.Games;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Boards;

public sealed class LeaderboardCalculatorTests
{
    private static readonly BoardKey Snake = new(GameKind.Snake, null, null);

    [Fact]
    public void Snake_ranks_each_player_by_their_best_score()
    {
        var board = LeaderboardCalculator.For(Snake,
        [
            P.Snake("ana", 120, 1), P.Snake("ana", 300, 2),
            P.Snake("marko", 250, 3),
            P.Snake("bojan", 80, 4)
        ], me: "marko");

        Assert.Equal(["ana", "marko", "bojan"], board.Rows.Select(r => r.Player));
        Assert.Equal([1, 2, 3], board.Rows.Select(r => r.Rank));
        Assert.Equal(300, board.Rows[0].Value);
        Assert.Equal(2, board.Rows[0].Games);
        Assert.True(board.Rows[1].IsMe);
    }

    [Fact]
    public void A_tie_goes_to_whoever_got_there_first()
    {
        var board = LeaderboardCalculator.For(Snake,
            [P.Snake("late", 200, 9), P.Snake("early", 200, 5), P.Snake("early", 200, 10)], me: "x");

        Assert.Equal(["early", "late"], board.Rows.Select(r => r.Player));
    }

    [Fact]
    public void Memory_ranks_fewest_moves_then_fastest_and_only_its_own_size()
    {
        var sixPairs = new BoardKey(GameKind.Memory, null, 6);

        var board = LeaderboardCalculator.For(sixPairs,
        [
            P.Memory("ana", 6, moves: 10, seconds: 40, minute: 1),
            P.Memory("marko", 6, moves: 10, seconds: 25, minute: 2),
            P.Memory("bojan", 6, moves: 9, seconds: 90, minute: 3),
            P.Memory("bojan", 8, moves: 4, seconds: 5, minute: 4)  // other size: ignored
        ], me: "x");

        Assert.Equal(["bojan", "marko", "ana"], board.Rows.Select(r => r.Player));
        Assert.Equal(9, board.Rows[0].Value);
        Assert.Equal(90, board.Rows[0].Tiebreak);
    }

    [Fact]
    public void Wins_boards_rank_by_wins_then_win_rate_per_difficulty()
    {
        var hard = new BoardKey(GameKind.TicTacToe, Difficulty.Hard, null);

        var board = LeaderboardCalculator.For(hard,
        [
            P.TicTacToe("ana", GameOutcome.Win, 1), P.TicTacToe("ana", GameOutcome.Loss, 2),
            P.TicTacToe("marko", GameOutcome.Win, 3),
            P.TicTacToe("bojan", GameOutcome.Win, 4), P.TicTacToe("bojan", GameOutcome.Win, 5),
            P.TicTacToe("easy-only", GameOutcome.Win, 6, Difficulty.Easy)
        ], me: "x");

        Assert.Equal(["bojan", "marko", "ana"], board.Rows.Select(r => r.Player));
        Assert.Equal(100, board.Rows[1].Tiebreak); // marko 1/1
        Assert.Equal(50, board.Rows[2].Tiebreak);  // ana 1/2
    }

    [Fact]
    public void An_empty_board_has_no_rows()
    {
        Assert.Empty(LeaderboardCalculator.For(Snake, [], me: "x").Rows);
    }
}
